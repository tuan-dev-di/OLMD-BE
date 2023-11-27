using System;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.LogicHandle;
using OptimizingLastMile.Models.Requests.Traffics;
using OptimizingLastMile.Models.Response.Orders;
using OptimizingLastMile.Models.Response.Traffics;

namespace OptimizingLastMile.Services.Maps;

public interface IMapService
{
    Task<List<DistanceResponse>> GetDistance(List<DistanceRequestPayload> payloads);
    Task<OrderBestRouteWrapperResponse> GetDistanceDuration(
        double originLat,
        double originLng,
        List<OrderInformation> orders,
        bool useDuration);
    Task<OrderDirectionResponse> GetDirection(double originLat, double originLng, double destinationLat, double destinationLng);
    Task<OrderBestRouteWrapperResponse> GetMinDurationRandom(
        double originLat,
        double originLng,
        List<OrderInformation> orders);
}