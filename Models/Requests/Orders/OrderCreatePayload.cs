using System;
using System.ComponentModel.DataAnnotations;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Requests.Orders;

public class OrderCreatePayload
{
    [Required]
    public long? OwnerId { get; set; }

    public long? DriverId { get; set; }

    [Required]
    [StringLength(200)]
    public string RecipientName { get; set; }

    [Required]
    [Phone]
    [StringLength(11)]
    public string RecipientPhoneNumber { get; set; }

    [Required]
    [StringLength(200)]
    public string ShippingProvince { get; set; }

    [Required]
    [StringLength(200)]
    public string ShippingDistrict { get; set; }

    [Required]
    [StringLength(200)]
    public string ShippingWard { get; set; }

    [Required]
    [StringLength(200)]
    public string ShippingAddress { get; set; }

    [Required]
    public double? Lat { get; set; }

    [Required]
    public double? Lng { get; set; }

    [Required]
    public DateTime? ExpectedShippingDate { get; set; }

    [Required]
    [StringLength(200)]
    public string SenderName { get; set; }

    [Required]
    [Phone]
    [StringLength(11)]
    public string SenderPhoneNumber { get; set; }

    public string Note { get; set; }
}