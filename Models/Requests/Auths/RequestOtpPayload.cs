using System;
using System.ComponentModel.DataAnnotations;

namespace OptimizingLastMile.Models.Requests.Auths;

public class RequestOtpPayload
{
    [Required]
    [StringLength(11)]
    [Phone]
    public string PhoneNumber { get; set; }
}