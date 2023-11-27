using System;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Commons;
using OptimizingLastMile.Models.Requests.AccountProfiles;
using OptimizingLastMile.Models.Requests.Drivers;

namespace OptimizingLastMile.Services.Accounts;

public interface IAccountService
{
    Task<Account> GetByUsername(string username);
    Task<Account> GetByPhoneNumber(string phoneNumber);
    Task<Account> GetByEmail(string email);
    Task<GenericResult<Account>> CreateManagerAcc(string username,
        string password,
        string name,
        DateTime? birthDay,
        string province,
        string district,
        string ward,
        string address,
        string phoneContact);
    Task<GenericResult<Account>> RegisterByUsername(string username, string password, RoleEnum role);
    Task<GenericResult> UpdateProfile(Account account, ProfileUpdatePayload payload);
    Task<GenericResult> UpdateDriverProfile(Account account, DriverProfileUpdatePayload payload);
    Task<Account> RegisterByPhonenumber(string phoneNumber, RoleEnum role);
    Task<Account> RegisterByEmail(string email, RoleEnum role);
}

