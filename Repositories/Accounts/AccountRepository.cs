using System;
using OptimizingLastMile.Repositories.Base;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Configs;
using Microsoft.EntityFrameworkCore;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Utils;
using OptimizingLastMile.Models.Response.AccountProfile;

namespace OptimizingLastMile.Repositories.Accounts;

public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    private readonly OlmDbContext _dbContext;

    public AccountRepository(OlmDbContext dbContext) : base(dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Account> GetByUsername(string username)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task<Account> GetByPhoneNumber(string phoneNumber)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
    }

    public async Task<Account> GetByEmail(string email)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<Account> GetByIdIncludeProfile(long id)
    {
        return await _dbContext.Accounts
            .Include(a => a.AccountProfile)
            .Include(a => a.DriverProfile)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Pagination<Account>> GetPaginationAccountIncludeProfile(string name, RoleEnum role, int pageNumber, int pageSize)
    {
        var query = _dbContext.Accounts
            .Include(a => a.DriverProfile)
            .Include(a => a.AccountProfile)
            .Where(a => a.Role == role);

        if (!string.IsNullOrWhiteSpace(name))
        {
            if (role == RoleEnum.DRIVER)
            {
                query = query.Where(a => a.DriverProfile.Name.Contains(name));
            }
            else
            {
                query = query.Where(a => a.AccountProfile.Name.Contains(name));
            }
        }

        return await Pagination<Account>.CreateAsync(query, pageNumber, pageSize);
    }

    public async Task<Account> GetAccountIncludeOrderShipping(long id)
    {
        return await _dbContext.Accounts
            .Include(a => a.OrderReceived.Where(o => o.CurrentOrderStatus == OrderStatusEnum.SHIPPING))
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Account> GetAccountIncludeOrderCreatedShipping(long id)
    {
        return await _dbContext.Accounts
            .Include(a => a.OrderCreated.Where(o => o.CurrentOrderStatus == OrderStatusEnum.SHIPPING))
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<AccountMinResponse>> GetAccountMin(RoleEnum role)
    {
        var query = _dbContext.Accounts.Where(a => a.Role == role && a.Status == StatusEnum.ACTIVE)
            .Include(a => a.AccountProfile)
            .Include(a => a.DriverProfile);

        if (role == RoleEnum.DRIVER)
        {
            return await query.Select(a => new AccountMinResponse
            {
                Id = a.Id,
                Name = a.DriverProfile.Name
            }).ToListAsync();
        }

        return await query.Select(a => new AccountMinResponse
        {
            Id = a.Id,
            Name = a.AccountProfile.Name
        }).ToListAsync();
    }

    public async Task<Account> GetAccountIncludeOwnershipOrder(long id)
    {
        return await _dbContext.Accounts.Include(a => a.OwnershipOrder.Where(o => o.CurrentOrderStatus != OrderStatusEnum.DELETED))
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Account>> GetAccountsByRoleAndStatus(RoleEnum role, StatusEnum status)
    {
        return await _dbContext.Accounts.Where(a => a.Role == role && a.Status == status).ToListAsync();
    }
}

