using System;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Models.Response.AccountProfile;
using OptimizingLastMile.Repositories.Base;
using OptimizingLastMile.Utils;

namespace OptimizingLastMile.Repositories.Accounts;

public interface IAccountRepository : IBaseRepository<Account>
{
    Task<Account> GetByUsername(string username);
    Task<Account> GetByIdIncludeProfile(long id);
    Task<Pagination<Account>> GetPaginationAccountIncludeProfile(string name, RoleEnum role, int pageNumber, int pageSize);
    Task<Account> GetAccountIncludeOrderShipping(long id);
    Task<Account> GetAccountIncludeOrderCreatedShipping(long id);
    Task<List<AccountMinResponse>> GetAccountMin(RoleEnum role);
    Task<Account> GetAccountIncludeOwnershipOrder(long id);
    Task<Account> GetByPhoneNumber(string phoneNumber);
    Task<Account> GetByEmail(string email);
    Task<List<Account>> GetAccountsByRoleAndStatus(RoleEnum role, StatusEnum status);
}

