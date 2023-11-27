using System.ComponentModel.DataAnnotations;

namespace OptimizingLastMile.Models.Params.Traffics;

public class OrderTrafficRandomParam
{
    [Required]
    public double? originLat { get; set; }

    [Required]
    public double? originLng { get; set; }
}