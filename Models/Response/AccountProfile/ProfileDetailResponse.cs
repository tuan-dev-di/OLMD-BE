using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Response.AccountProfile;

public class ProfileDetailResponse
{
    public long Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public RoleEnum Role { get; set; }
    public StatusEnum Status { get; set; }

    public AccountDetailProfileResponse AccountProfile { get; set; }
}

