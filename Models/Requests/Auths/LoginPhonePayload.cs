using System;
using System.ComponentModel.DataAnnotations;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Requests.Auths;

public class LoginPhonePayload
{
    [Required]
    [StringLength(11)]
    [Phone]
    public string PhoneNumber { get; set; }

    public string Otp { get; set; }

    public RoleEnum Role { get; set; }
}