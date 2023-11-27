using System;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.Requests.Orders;

namespace OptimizingLastMile.Services.Orders;

public interface IOrderService
{
    Task<GenericResult<OrderInformation>> CreateOrder(OrderCreatePayload payload, long creatorId);
    Task<GenericResult> UpdateOrder(OrderInformation order, OrderUpdatePayload payload);
    Task<GenericResult> DeleteOrder(OrderInformation order);
    Task<GenericResult> UpdateOrderStatusByDriver(OrderStatusEnum status, OrderInformation order, string description);
    Task<GenericResult> UpdateOrderStatusByManager(OrderStatusEnum status, OrderInformation order);
}

