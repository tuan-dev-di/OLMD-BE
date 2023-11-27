using System;
using System.ComponentModel.DataAnnotations;

namespace OptimizingLastMile.Models.Params.Traffics;

public class OrderTrafficParam
{
    public bool useDuration { get; set; } = false;

    [Required]
    public double? originLat { get; set; }

    [Required]
    public double? originLng { get; set; }
}