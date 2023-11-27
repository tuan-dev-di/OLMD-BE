using System;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Hubs;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.LogicHandle;
using OptimizingLastMile.Models.Requests.Orders;
using OptimizingLastMile.Models.Response.Orders;
using OptimizingLastMile.Repositories.Accounts;
using OptimizingLastMile.Repositories.Notifications;
using OptimizingLastMile.Repositories.Orders;

namespace OptimizingLastMile.Services.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository,
        IAccountRepository accountRepository,
        INotificationRepository notificationRepository,
        IMapper mapper)
    {
        this._orderRepository = orderRepository;
        this._accountRepository = accountRepository;
        this._notificationRepository = notificationRepository;
        this._mapper = mapper;
    }

    public async Task<GenericResult<OrderInformation>> CreateOrder(OrderCreatePayload payload, long creatorId)
    {
        var owner = await _accountRepository.GetById(payload.OwnerId);

        if (owner is null)
        {
            var error = Errors.Order.InvalidOwner();
            return GenericResult<OrderInformation>.Fail(error);
        }

        if (owner.Role != RoleEnum.CUSTOMER)
        {
            var error = Errors.Order.InvalidOwner();
            return GenericResult<OrderInformation>.Fail(error);
        }

        if (payload.DriverId.HasValue)
        {
            var driver = await _accountRepository.GetById(payload.DriverId);

            if (driver is null || driver.Role != RoleEnum.DRIVER)
            {
                var error = Errors.Order.InvalidDriver();
                return GenericResult<OrderInformation>.Fail(error);
            }
        }

        var order = new OrderInformation
        {
            Id = Guid.NewGuid(),

            OwnerId = owner.Id,
            CreatorId = creatorId,
            DriverId = payload.DriverId,

            RecipientName = payload.RecipientName,
            RecipientPhoneNumber = payload.RecipientPhoneNumber,

            ShippingProvince = payload.ShippingProvince,
            ShippingDistrict = payload.ShippingDistrict,
            ShippingWard = payload.ShippingWard,
            ShippingAddress = payload.ShippingAddress,

            Lat = payload.Lat.Value,
            Lng = payload.Lng.Value,

            ExpectedShippingDate = payload.ExpectedShippingDate,

            SenderName = payload.SenderName,
            SenderPhoneNumber = payload.SenderPhoneNumber,

            Note = payload.Note,

            CurrentOrderStatus = OrderStatusEnum.CREATED
        };

        var orderAuditCreated = new OrderAudit
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
            OrderStatus = OrderStatusEnum.CREATED
        };

        order.AddOrderAudit(orderAuditCreated);

        if (order.DriverId.HasValue)
        {
            var orderAuditProcessing = new OrderAudit
            {
                CreatedDate = DateTime.UtcNow,
                OrderStatus = OrderStatusEnum.PROCESSING,

                DriverId = order.DriverId
            };

            order.CurrentOrderStatus = OrderStatusEnum.PROCESSING;
            order.AddOrderAudit(orderAuditProcessing);
        }

        _orderRepository.Create(order);
        await _orderRepository.SaveAsync();

        if (order.CurrentOrderStatus == OrderStatusEnum.PROCESSING && order.DriverId is not null)
        {
            var noti = new NotificationLog
            {
                NotificationType = NotificationTypeEnum.ASSIGNED_ORDER,
                DriverId = order.DriverId,
                ReceiverId = order.DriverId,
                CreatedDate = DateTime.UtcNow,
                OrderId = order.Id,
                IsRead = false
            };

            _notificationRepository.Create(noti);
            await _notificationRepository.SaveAsync();
        }

        return GenericResult<OrderInformation>.Ok(order);
    }

    public async Task<GenericResult> UpdateOrder(OrderInformation order, OrderUpdatePayload payload)
    {
        if (order.CurrentOrderStatus == OrderStatusEnum.PICK_OFF ||
            order.CurrentOrderStatus == OrderStatusEnum.SHIPPING ||
            order.CurrentOrderStatus == OrderStatusEnum.DELIVERED ||
            order.CurrentOrderStatus == OrderStatusEnum.DELETED)
        {
            var error = Errors.Order.NotAllowUpdateOrderForCurrentStatus();
            return GenericResult.Fail(error);
        }

        if (payload.DriverId.HasValue)
        {
            var driver = await _accountRepository.GetById(payload.DriverId);

            var error = Errors.Order.InvalidDriver();

            if (driver is null || driver.Role != RoleEnum.DRIVER)
            {
                return GenericResult.Fail(error);
            }
        }

        var oldDriverId = order.DriverId;

        _mapper.Map(payload, order);

        if (order.CurrentOrderStatus != OrderStatusEnum.PROCESSING && order.DriverId.HasValue)
        {
            var orderAudit = new OrderAudit
            {
                OrderStatus = OrderStatusEnum.PROCESSING,
                CreatedDate = DateTime.UtcNow,
                DriverId = order.DriverId,
                OrderId = order.Id
            };

            order.AddOrderAudit(orderAudit);
            order.CurrentOrderStatus = OrderStatusEnum.PROCESSING;
        }

        await _orderRepository.SaveAsync();

        var newDriverId = order.DriverId;

        if (order.CurrentOrderStatus == OrderStatusEnum.PROCESSING && oldDriverId != newDriverId)
        {
            var noti = new NotificationLog
            {
                NotificationType = NotificationTypeEnum.ASSIGNED_ORDER,
                DriverId = newDriverId,
                IsRead = false,
                CreatedDate = DateTime.UtcNow,
                OrderId = order.Id,
                ReceiverId = newDriverId
            };

            _notificationRepository.Create(noti);
            await _notificationRepository.SaveAsync();
        }

        return GenericResult.Ok();
    }

    public async Task<GenericResult> DeleteOrder(OrderInformation order)
    {
        if (order.CurrentOrderStatus != OrderStatusEnum.CREATED &&
            order.CurrentOrderStatus != OrderStatusEnum.PROCESSING &&
            order.CurrentOrderStatus != OrderStatusEnum.DELIVERY_FAILED &&
            order.CurrentOrderStatus != OrderStatusEnum.DELETED)
        {
            var error = Errors.Order.NotAllowDeleteOrderForCurrentStatus();
            return GenericResult.Fail(error);
        }

        order.CurrentOrderStatus = OrderStatusEnum.DELETED;

        var orderAudit = new OrderAudit
        {
            CreatedDate = DateTime.UtcNow,
            OrderStatus = OrderStatusEnum.DELETED
        };

        order.AddOrderAudit(orderAudit);

        await _orderRepository.SaveAsync();

        return GenericResult.Ok();
    }

    public async Task<GenericResult> UpdateOrderStatusByManager(OrderStatusEnum status, OrderInformation order)
    {
        if (status != OrderStatusEnum.PROCESSING && status != OrderStatusEnum.DELETED
            && order.CurrentOrderStatus != OrderStatusEnum.DELIVERY_FAILED)
        {
            var error = Errors.Order.RoleNotAllowUpdateThisStatus(status);
            return GenericResult.Fail(error);
        }

        var audit = new OrderAudit
        {
            CreatedDate = DateTime.UtcNow,
            OrderStatus = status,

            DriverId = order.DriverId
        };

        order.CurrentOrderStatus = status;
        order.AddOrderAudit(audit);

        await _orderRepository.SaveAsync();

        return GenericResult.Ok();
    }

    public async Task<GenericResult> UpdateOrderStatusByDriver(OrderStatusEnum status, OrderInformation order, string description)
    {
        if (status != OrderStatusEnum.PICK_OFF &&
            status != OrderStatusEnum.SHIPPING &&
            status != OrderStatusEnum.DELIVERED &&
            status != OrderStatusEnum.DELIVERY_FAILED &&
            order.CurrentOrderStatus != OrderStatusEnum.CREATED &&
            order.CurrentOrderStatus != OrderStatusEnum.DELIVERY_FAILED &&
            order.CurrentOrderStatus != OrderStatusEnum.DELETED)
        {
            var error = Errors.Order.RoleNotAllowUpdateThisStatus(status);
            return GenericResult.Fail(error);
        }

        switch (status)
        {
            case OrderStatusEnum.PICK_OFF:
                if (order.CurrentOrderStatus != OrderStatusEnum.PROCESSING)
                {
                    var error = Errors.Order.InvalidUpdateOrderStatus();
                    return GenericResult.Fail(error);
                }
                break;
            case OrderStatusEnum.SHIPPING:
                if (order.CurrentOrderStatus != OrderStatusEnum.PICK_OFF)
                {
                    var error = Errors.Order.InvalidUpdateOrderStatus();
                    return GenericResult.Fail(error);
                }
                break;
            case OrderStatusEnum.DELIVERED:
                if (order.CurrentOrderStatus != OrderStatusEnum.SHIPPING)
                {
                    var error = Errors.Order.InvalidUpdateOrderStatus();
                    return GenericResult.Fail(error);
                }
                break;
            case OrderStatusEnum.DELIVERY_FAILED:
                if (order.CurrentOrderStatus != OrderStatusEnum.SHIPPING)
                {
                    var error = Errors.Order.InvalidUpdateOrderStatus();
                    return GenericResult.Fail(error);
                }
                break;
            default:
                break;
        }

        if (status == OrderStatusEnum.DELIVERY_FAILED)
        {
            var audit = new OrderAudit
            {
                CreatedDate = DateTime.UtcNow,
                OrderStatus = status,
                Description = description,

                DriverId = order.DriverId
            };

            order.CurrentOrderStatus = status;
            order.AddOrderAudit(audit);
        }
        else
        {
            var audit = new OrderAudit
            {
                CreatedDate = DateTime.UtcNow,
                OrderStatus = status,

                DriverId = order.DriverId
            };

            order.CurrentOrderStatus = status;
            order.AddOrderAudit(audit);
        }

        await _orderRepository.SaveAsync();

        return GenericResult.Ok();
    }
}

