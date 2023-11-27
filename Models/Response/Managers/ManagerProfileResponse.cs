using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Response.Managers;

public class ManagerProfileResponse
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public DateTime? BirthDay { get; set; }
    public string Province { get; set; }
    public string District { get; set; }
    public string Ward { get; set; }
    public string Address { get; set; }
    public string PhoneContact { get; set; }
    public StatusEnum Status { get; set; }
}

