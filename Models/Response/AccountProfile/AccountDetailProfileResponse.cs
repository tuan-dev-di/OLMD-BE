using System;
namespace OptimizingLastMile.Models.Response.AccountProfile;

public class AccountDetailProfileResponse
{
    public string Name { get; set; }
    public DateTime? BirthDay { get; set; }
    public string Province { get; set; }
    public string District { get; set; }
    public string Ward { get; set; }
    public string Address { get; set; }
    public string PhoneContact { get; set; }
}