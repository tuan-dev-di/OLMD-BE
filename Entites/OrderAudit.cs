using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Entites;

public class OrderAudit
{
    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }
    public string Description { get; set; }
    public OrderStatusEnum OrderStatus { get; set; }

    public Guid OrderId { get; set; }
    public long? DriverId { get; set; }
}