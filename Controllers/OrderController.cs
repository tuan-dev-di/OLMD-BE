using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Hubs;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.Params.Orders;
using OptimizingLastMile.Models.Params.Traffics;
using OptimizingLastMile.Models.Requests.Feedbacks;
using OptimizingLastMile.Models.Requests.Orders;
using OptimizingLastMile.Models.Response.Notifications;
using OptimizingLastMile.Models.Response.Orders;
using OptimizingLastMile.Repositories.Accounts;
using OptimizingLastMile.Repositories.FeedBacks;
using OptimizingLastMile.Repositories.Notifications;
using OptimizingLastMile.Repositories.Orders;
using OptimizingLastMile.Services.Audits;
using OptimizingLastMile.Services.Maps;
using OptimizingLastMile.Services.Orders;
using OptimizingLastMile.Utils;

namespace OptimizingLastMile.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IOrderService _orderService;
    private readonly IAuditService _auditService;
    private readonly IMapService _mapService;
    private readonly IOrderRepository _orderRepository;
    private readonly IFeedBackRepository _feedBackRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public OrderController(IAccountRepository accountRepository,
        IOrderService orderService,
        IAuditService auditService,
        IMapService mapService,
        IOrderRepository orderRepository,
        IFeedBackRepository feedBackRepository,
        INotificationRepository notificationRepository,
        IMapper mapper,
        IHubContext<NotificationHub> notificationHub)
    {
        this._accountRepository = accountRepository;
        this._orderService = orderService;
        this._auditService = auditService;
        this._mapService = mapService;
        this._orderRepository = orderRepository;
        this._feedBackRepository = feedBackRepository;
        this._notificationRepository = notificationRepository;
        this._mapper = mapper;
        this._notificationHub = notificationHub;
    }

    [HttpPost]
    [Authorize(Roles = "MANAGER")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreatePayload payload)
    {
        var creatorId = MyTools.GetUserOfRequest(User.Claims);

        var createResult = await _orderService.CreateOrder(payload, creatorId);

        if (createResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(createResult.Error));
        }

        var order = createResult.Data;

        return CreatedAtAction(nameof(GetOrderDetail), new { id = order.Id }, null);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "CUSTOMER,DRIVER,MANAGER")]
    public async Task<IActionResult> GetOrderDetail([FromRoute] Guid id)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var author = await _accountRepository.GetById(authorId);

        var order = await _orderRepository.GetOrderDetail(id);

        if (order is null)
        {
            return NotFound();
        }

        switch (author.Role)
        {
            case RoleEnum.MANAGER:
                {
                    if (order.CreatorId != author.Id)
                    {
                        return Forbid();
                    }
                    break;
                }
            case RoleEnum.DRIVER:
                {
                    if (order.DriverId != author.Id)
                    {
                        return Forbid();
                    }
                    break;
                }
            case RoleEnum.CUSTOMER:
                {
                    if (order.OwnerId != author.Id)
                    {
                        return Forbid();
                    }
                    break;
                }
            default:
                {
                    return Forbid();
                }
        }

        var dataRes = _mapper.Map<OrderDetailResponse>(order);

        return Ok(EnvelopResponse.Ok(dataRes));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "MANAGER")]
    public async Task<IActionResult> UpdateOrder([FromRoute] Guid id, [FromBody] OrderUpdatePayload payload)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);
        var order = await _orderRepository.GetOrderIncludeAudit(id);

        if (order is null)
        {
            return NotFound();
        }

        if (order.CreatorId != authorId)
        {
            return Forbid();
        }

        var updateResult = await _orderService.UpdateOrder(order, payload);

        if (updateResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(updateResult.Error));
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "MANAGER")]
    public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);
        var order = await _orderRepository.GetOrderIncludeAudit(id);

        if (order is null)
        {
            return BadRequest();
        }

        if (order.CreatorId != authorId)
        {
            return Forbid();
        }

        var deleteResult = await _orderService.DeleteOrder(order);

        if (deleteResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(deleteResult.Error));
        }

        return NoContent();
    }

    [HttpPost("{id}/feedbacks")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> FeedBackOrder([FromRoute] Guid id, [FromBody] FeedBackCreatePayload payload)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var order = await _orderRepository.GetById(id);

        if (order is null)
        {
            return NotFound();
        }

        if (order.OwnerId != authorId)
        {
            return Forbid();
        }

        if (order.CurrentOrderStatus != OrderStatusEnum.DELIVERED)
        {
            var error = Errors.Order.NotAllowFeedback();
            return BadRequest(EnvelopResponse.Error(error));
        }

        var feedback = _mapper.Map<Feedback>(payload);

        feedback.CustomerId = authorId;
        feedback.DriverId = order.DriverId.Value;
        feedback.OrderId = order.Id;

        _feedBackRepository.Create(feedback);
        await _feedBackRepository.SaveAsync();

        return Ok();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "DRIVER,MANAGER")]
    public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid id, [FromBody] OrderStatusUpdatePayload payload)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);
        var account = await _accountRepository.GetById(authorId);

        var order = await _orderRepository.GetOrderIncludeAudit(id);

        if (order is null)
        {
            return NotFound();
        }

        GenericResult updateResult;

        if (account.Role == RoleEnum.MANAGER)
        {
            updateResult = await _orderService.UpdateOrderStatusByManager(payload.Status, order);
        }
        else
        {
            updateResult = await _orderService.UpdateOrderStatusByDriver(payload.Status, order, payload.Description);

            if (order.CurrentOrderStatus == OrderStatusEnum.DELIVERED)
            {
                var notiForManager = new NotificationLog
                {
                    NotificationType = NotificationTypeEnum.DELIVERY_ORDER_SUCCESSFUL,
                    IsRead = false,
                    OrderId = order.Id,
                    DriverId = order.DriverId,
                    ReceiverId = order.CreatorId,
                    CreatedDate = DateTime.UtcNow
                };


                var notiForCustomer = new NotificationLog
                {
                    NotificationType = NotificationTypeEnum.DELIVERY_ORDER_SUCCESSFUL,
                    IsRead = false,
                    OrderId = order.Id,
                    DriverId = order.DriverId,
                    ReceiverId = order.OwnerId,
                    CreatedDate = DateTime.UtcNow
                };

                _notificationRepository.Create(notiForManager);
                _notificationRepository.Create(notiForCustomer);

                await _notificationRepository.SaveAsync();

                var notiToSent = _mapper.Map<NotificationResponse>(notiForManager);

                // Sent notification
                var groupName = MyTools.GetGroupName(order.CreatorId);
                await _notificationHub.Clients.Group(groupName).SendAsync(GlobalConstant.WEBSOCKET_METHOD, notiToSent);
            }
        }

        if (updateResult.IsFail)
        {
            return BadRequest(EnvelopResponse.Error(updateResult.Error));
        }

        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "CUSTOMER,MANAGER,DRIVER")]
    public async Task<IActionResult> GetOrder([FromQuery] OrderParam param)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var authorAcc = await _accountRepository.GetById(authorId);

        if (authorAcc.Status != StatusEnum.ACTIVE)
        {
            return Forbid();
        }

        Pagination<OrderInformation> orders;

        if (authorAcc.Role == RoleEnum.MANAGER)
        {
            orders = await _orderRepository.GetOrderForManager(authorId, param.SearchName, param.StartDate, param.EndDate, param.Status, param.Limit, param.Page);
        }
        else if (authorAcc.Role == RoleEnum.DRIVER)
        {
            orders = await _orderRepository.GetOrderForDriver(authorId, param.StartDate, param.EndDate, param.Status, param.Limit, param.Page);
        }
        else
        {
            orders = await _orderRepository.GetOrderForCustomer(authorId, param.StartDate, param.EndDate, param.Status, param.Limit, param.Page);
        }

        var resData = _mapper.Map<MultiObjectResponse<OrderResponse>>(orders);

        return Ok(EnvelopResponse.Ok(resData));
    }

    [HttpGet("{id}/audit")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> GetAuditOfOrder([FromRoute] Guid id)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);
        var order = await _orderRepository.GetOrderIncludeAudit(id);

        if (order is null)
        {
            return NotFound();
        }

        if (order.OwnerId != authorId)
        {
            return Forbid();
        }

        var orderHistory = _auditService.BuildOrderHistory(order);

        return Ok(EnvelopResponse.Ok(orderHistory));
    }

    [HttpGet("routes")]
    [Authorize(Roles = "DRIVER")]
    public async Task<IActionResult> GetRouteByDistanceDuration([FromQuery] OrderTrafficParam param)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var orders = await _orderRepository.GetOrderShippingInDay(authorId);

        var distanceDuration = await _mapService.GetDistanceDuration(param.originLat.Value, param.originLng.Value, orders, param.useDuration);

        return Ok(EnvelopResponse.Ok(distanceDuration));
    }

    [HttpGet("directions")]
    [Authorize(Roles = "DRIVER")]
    public async Task<IActionResult> GetDirection([FromQuery] OrderDirectionParam param)
    {
        var directionResponse = await _mapService.GetDirection(param.originLat.Value, param.originLng.Value, param.destinationLat.Value, param.destinationLng.Value);

        return Ok(EnvelopResponse.Ok(directionResponse));
    }

    [HttpGet("routes/random")]
    [Authorize(Roles = "DRIVER")]
    public async Task<IActionResult> GetRouteRandom([FromQuery] OrderTrafficRandomParam param)
    {
        var authorId = MyTools.GetUserOfRequest(User.Claims);

        var orders = await _orderRepository.GetOrderShippingInDay(authorId);

        var dataRes = await _mapService.GetMinDurationRandom(param.originLat.Value, param.originLng.Value, orders);

        return Ok(EnvelopResponse.Ok(dataRes));
    }
}

