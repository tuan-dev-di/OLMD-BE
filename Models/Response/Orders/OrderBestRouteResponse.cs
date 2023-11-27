using System;
namespace OptimizingLastMile.Models.Response.Orders;

public class OrderBestRouteResponse
{
    public int No { get; set; }
    public OrderResponse Order { get; set; }
}

public class OrderBestRouteWrapperResponse
{
    public long TotalTimeTravel { get; set; }
    public List<OrderBestRouteResponse> ListRoute { get; set; }
}