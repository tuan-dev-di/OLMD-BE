using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Params.Orders;

public class OrderParam : ResourceParam
{
    public string? SearchName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public OrderStatusEnum? Status { get; set; }
}