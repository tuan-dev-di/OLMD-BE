using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Entites;

public class OrderInformation
{
    public Guid Id { get; set; }

    public long OwnerId { get; set; }
    public long CreatorId { get; set; }
    public long? DriverId { get; set; }

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

    public Account Owner { get; set; }
    public Account Driver { get; set; }

    public List<OrderAudit> OrderAudits { get; private set; }

    public void AddOrderAudit(OrderAudit orderAudit)
    {
        if (OrderAudits is null)
        {
            OrderAudits = new List<OrderAudit>();
        }

        OrderAudits.Add(orderAudit);
    }
}