using System;
using System.ComponentModel.DataAnnotations;

namespace OptimizingLastMile.Models.Params.Traffics;

public class OrderDirectionParam
{
    [Required]
    public double? originLat { get; set; }

    [Required]
    public double? originLng { get; set; }

    [Required]
    public double? destinationLat { get; set; }

    [Required]
    public double? destinationLng { get; set; }
}