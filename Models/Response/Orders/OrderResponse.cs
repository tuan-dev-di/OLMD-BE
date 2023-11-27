using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Response.Orders;

public class OrderResponse
{
    public Guid Id { get; set; }

    public OrderActorResponse Owner { get; set; }
    public OrderActorResponse Driver { get; set; }

    public long CreatorId { get; set; }

    public string ShippingProvince { get; set; }
    public string ShippingDistrict { get; set; }
    public string ShippingWard { get; set; }
    public string ShippingAddress { get; set; }

    public DateTime? ExpectedShippingDate { get; set; }

    public OrderStatusEnum CurrentOrderStatus { get; set; }
}