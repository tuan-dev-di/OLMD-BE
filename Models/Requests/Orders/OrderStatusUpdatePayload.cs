using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Requests.Orders;

public class OrderStatusUpdatePayload
{
    public OrderStatusEnum Status { get; set; }
    public string Description { get; set; }
}