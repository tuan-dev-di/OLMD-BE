using System;
using System.ComponentModel.DataAnnotations;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Requests.Auths;

public class RegisterByUsernamePayload
{
    [Required]
    public string Username { get; set; }

    [Required]
    [MinLength(8)]
    [MaxLength(50)]
    public string Password { get; set; }

    [Required]
    public RoleEnum Role { get; set; }
}

