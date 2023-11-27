using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Response.Orders;

public class OrderDetailResponse
{
    public Guid Id { get; set; }

    public OrderActorResponse Owner { get; set; }
    public OrderActorResponse Driver { get; set; }

    public string RecipientName { get; set; }
    public string RecipientPhoneNumber { get; set; }

    public string ShippingProvince { get; set; }
    public string ShippingDistrict { get; set; }
    public string ShippingWard { get; set; }
    public string ShippingAddress { get; set; }

    public double Lat { get; set; }
    public double Lng { get; set; }

    public DateTime? ExpectedShippingDate { get; set; }

    public DateTime? PickupDate { get; set; }
    public DateTime? DropoffDate { get; set; }

    public string SenderName { get; set; }
    public string SenderPhoneNumber { get; set; }

    public string Note { get; set; }

    public OrderStatusEnum CurrentOrderStatus { get; set; }
}

public class OrderActorResponse
{
    public long Id { get; set; }
    public RoleEnum Role { get; set; }
    public string Name { get; set; }
    public string PhoneContact { get; set; }
}