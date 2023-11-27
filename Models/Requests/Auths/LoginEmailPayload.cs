using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Requests.Auths;

public class LoginEmailPayload
{
    public string AccessToken { get; set; }
    public RoleEnum Role { get; set; }
}