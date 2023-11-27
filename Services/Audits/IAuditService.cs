using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.Response.Orders;

namespace OptimizingLastMile.Services.Audits;

public interface IAuditService
{
    List<OrderHistoryResponse> BuildOrderHistory(OrderInformation order);
}