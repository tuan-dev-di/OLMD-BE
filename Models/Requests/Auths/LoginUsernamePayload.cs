using System;
using System.ComponentModel.DataAnnotations;

namespace OptimizingLastMile.Models.Requests.Auths;

public class LoginUsernamePayload
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}