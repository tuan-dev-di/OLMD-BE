using System;
using System.ComponentModel.DataAnnotations;

namespace OptimizingLastMile.Models.Requests.Feedbacks;

public class FeedBackCreatePayload
{
    [Required]
    [Range(minimum: 1, maximum: 5)]
    public double? Rate { get; set; }

    [Required]
    public string Comment { get; set; }
}