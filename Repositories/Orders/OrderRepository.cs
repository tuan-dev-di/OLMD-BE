using System;
using Microsoft.EntityFrameworkCore;
using OptimizingLastMile.Configs;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Entites.Enums;
using OptimizingLastMile.Repositories.Base;
using OptimizingLastMile.Utils;

namespace OptimizingLastMile.Repositories.Orders;

public class OrderRepository : BaseRepository<OrderInformation>, IOrderRepository
{
    private readonly OlmDbContext _dbContext;

    public OrderRepository(OlmDbContext dbContext) : base(dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<OrderInformation> GetOrderDetail(Guid id)
    {
        return await _dbContext.OrderInformation
            .Include(o => o.Driver).ThenInclude(a => a.DriverProfile)
            .Include(o => o.Owner).ThenInclude(a => a.AccountProfile)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<OrderInformation> GetOrderIncludeAudit(Guid id)
    {
        return await _dbContext.OrderInformation.Include(o => o.OrderAudits)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Pagination<OrderInformation>> GetOrderForManager(
        long managerId,
        string searchName,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatusEnum? orderStatus,
        int pageSize,
        int pageNumber)
    {
        var query = _dbContext.OrderInformation
            .Include(o => o.Owner).ThenInclude(a => a.AccountProfile)
            .Include(o => o.Driver).ThenInclude(a => a.DriverProfile)
            .Where(o => o.CreatorId == managerId);

        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(o => o.Driver.DriverProfile.Name.Contains(searchName) ||
            o.Owner.AccountProfile.Name.Contains(searchName) ||
            o.RecipientName.Contains(searchName) ||
            o.SenderName.Contains(searchName));
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue &&
            o.ExpectedShippingDate.Value >= startDate.Value &&
            o.ExpectedShippingDate.Value <= endDate.Value);
        }
        else if (startDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue && o.ExpectedShippingDate.Value >= startDate.Value);
        }
        else if (endDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue && o.ExpectedShippingDate.Value <= endDate.Value);
        }

        if (orderStatus is not null)
        {
            query = query.Where(o => o.CurrentOrderStatus == orderStatus);
        }

        return await Pagination<OrderInformation>.CreateAsync(query, pageNumber, pageSize);
    }

    public async Task<Pagination<OrderInformation>> GetOrderForCustomer(
        long customerId,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatusEnum? orderStatus,
        int pageSize,
        int pageNumber)
    {
        var query = _dbContext.OrderInformation
            .Include(o => o.Owner).ThenInclude(a => a.AccountProfile)
            .Include(o => o.Driver).ThenInclude(a => a.DriverProfile)
            .Where(o => o.OwnerId == customerId && o.CurrentOrderStatus != OrderStatusEnum.DELETED);

        if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue &&
            o.ExpectedShippingDate.Value >= startDate.Value &&
            o.ExpectedShippingDate.Value <= endDate.Value);
        }
        else if (startDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue && o.ExpectedShippingDate.Value >= startDate.Value);
        }
        else if (endDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue && o.ExpectedShippingDate.Value <= endDate.Value);
        }

        if (orderStatus is not null)
        {
            query = query.Where(o => o.CurrentOrderStatus == orderStatus);
        }

        return await Pagination<OrderInformation>.CreateAsync(query, pageNumber, pageSize);
    }

    public async Task<Pagination<OrderInformation>> GetOrderForDriver(
        long driverId,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatusEnum? orderStatus,
        int pageSize,
        int pageNumber)
    {
        var query = _dbContext.OrderInformation
            .Include(o => o.Owner).ThenInclude(a => a.AccountProfile)
            .Include(o => o.Driver).ThenInclude(a => a.DriverProfile)
            .Where(o => o.DriverId == driverId && o.CurrentOrderStatus != OrderStatusEnum.DELETED);

        if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue &&
            o.ExpectedShippingDate.Value >= startDate.Value &&
            o.ExpectedShippingDate.Value <= endDate.Value);
        }
        else if (startDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue && o.ExpectedShippingDate.Value >= startDate.Value);
        }
        else if (endDate.HasValue)
        {
            query = query.Where(o => o.ExpectedShippingDate.HasValue && o.ExpectedShippingDate.Value <= endDate.Value);
        }

        if (orderStatus is not null)
        {
            query = query.Where(o => o.CurrentOrderStatus == orderStatus);
        }

        return await Pagination<OrderInformation>.CreateAsync(query, pageNumber, pageSize);
    }

    public async Task<List<OrderInformation>> GetOrderShippingInDay(long driverId)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return await _dbContext.OrderInformation
            .Include(o => o.Owner).ThenInclude(a => a.AccountProfile)
            .Include(o => o.Driver).ThenInclude(a => a.DriverProfile)
            .Where(o => o.DriverId == driverId &&
        (o.ExpectedShippingDate >= today && o.ExpectedShippingDate <= tomorrow)).ToListAsync();
    }
}

