using System;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using OptimizingLastMile.Configure;
using OptimizingLastMile.Entites;
using OptimizingLastMile.Models.LogicHandle;
using OptimizingLastMile.Models.Requests.Traffics;
using OptimizingLastMile.Models.Response.Orders;
using OptimizingLastMile.Models.Response.Traffics;
using static OptimizingLastMile.Models.Commons.Errors;

namespace OptimizingLastMile.Services.Maps;

public class MapService : IMapService
{
    private readonly string DISTANCE_UNIT = "meter";
    private readonly string DURATION_UNIT = "second";

    private readonly string ORIGINS_DISTANCE_KEY = "origins";
    private readonly string DESTINATIONS_DISTANCE_KEY = "destinations";

    private readonly string ORIGIN_DIRECTION_KEY = "origin";
    private readonly string DESTINATION_DIRECTION_KEY = "destination";

    private readonly string VEHICLE_KEY = "vehicle";
    private readonly string VEHICLE_VALUE = "bike";

    private readonly string API_KEY = "api_key";

    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly MapConfig _mapConfig;
    private readonly AlgorithmConfig _algorithmConfig;

    public MapService(HttpClient httpClient,
        IMapper mapper,
        IOptionsSnapshot<MapConfig> options,
        IOptionsSnapshot<AlgorithmConfig> algOptions)
    {
        _httpClient = httpClient;
        _mapper = mapper;
        _mapConfig = options.Value;
        _algorithmConfig = algOptions.Value;
    }

    public async Task<OrderDirectionResponse> GetDirection(double originLat, double originLng, double destinationLat, double destinationLng)
    {
        var origin = $"{originLat},{originLng}";
        var destination = $"{destinationLat},{destinationLng}";

        var url = new StringBuilder();

        url.Append(_mapConfig.DirectionUrl);
        url.Append('?');
        url.Append($"{ORIGIN_DIRECTION_KEY}={origin}");
        url.Append('&');
        url.Append($"{DESTINATION_DIRECTION_KEY}={destination}");
        url.Append('&');
        url.Append($"{VEHICLE_KEY}={VEHICLE_VALUE}");
        url.Append('&');
        url.Append($"{API_KEY}={_mapConfig.ApiKey}");

        var uri = new Uri(url.ToString());

        var directionResponse = await _httpClient.GetFromJsonAsync<OrderDirectionResponse>(uri);

        return directionResponse;
    }

    public async Task<List<DistanceResponse>> GetDistance(List<DistanceRequestPayload> payloads)
    {
        var distanceResponses = new List<DistanceResponse>();

        for (int i = 0; i < payloads.Count - 1; i++)
        {
            var url = new StringBuilder();

            var originInfo = payloads[i];
            var origin = $"{originInfo.Lat},{originInfo.Lng}";

            var listPairLatLng = payloads.Skip(i + 1).Select(a => $"{a.Lat},{a.Lng}").ToList();

            var destination = string.Join("|", listPairLatLng);

            url.Append(_mapConfig.DistanceUrl);
            url.Append('?');
            url.Append($"{ORIGINS_DISTANCE_KEY}={origin}");
            url.Append('&');
            url.Append($"{DESTINATIONS_DISTANCE_KEY}={destination}");
            url.Append('&');
            url.Append($"{VEHICLE_KEY}={VEHICLE_VALUE}");
            url.Append('&');
            url.Append($"{API_KEY}={_mapConfig.ApiKey}");

            var uri = new Uri(url.ToString());

            var distanceWrapper = await _httpClient.GetFromJsonAsync<DistanceWrapper>(uri);

            var elements = distanceWrapper.Rows.First().Elements;

            int tmpIndex = 0;

            var distances = payloads.Skip(i + 1).Select(a =>
            {
                var element = elements[tmpIndex++];

                var from = originInfo.Address;
                var fromLat = originInfo.Lat;
                var fromLng = originInfo.Lng;

                var to = a.Address;
                var toLat = a.Lat;
                var toLng = a.Lng;

                var distanceValue = element.Distance.Value;
                var durationValue = element.Duration.Value;

                return new DistanceResponse
                {
                    From = new AddressResponse
                    {
                        Address = from,
                        Lat = fromLat,
                        Lng = fromLng
                    },
                    To = new AddressResponse
                    {
                        Address = to,
                        Lat = toLat,
                        Lng = toLng
                    },
                    Distance = new DistanceDurationResponse
                    {
                        Value = distanceValue,
                        Unit = DISTANCE_UNIT
                    },
                    Duration = new DistanceDurationResponse
                    {
                        Value = durationValue,
                        Unit = DURATION_UNIT
                    }
                };
            }).ToList();

            distanceResponses.AddRange(distances);
        }

        return distanceResponses;
    }

    public async Task<OrderBestRouteWrapperResponse> GetDistanceDuration(
        double originLat,
        double originLng,
        List<OrderInformation> orders,
        bool useDuration)
    {
        int no = 1;
        var result = new OrderBestRouteWrapperResponse();
        result.TotalTimeTravel = 0;
        result.ListRoute = new List<OrderBestRouteResponse>();

        if (orders is null || orders.Count == 0)
        {
            return result;
        }

        var manipulateOrders = orders.ToList();

        // Get distance/duration from origin to other orders
        // Create origin lat, lng
        var originLatLng = $"{originLat},{originLng}";

        // Get list lat, lng of all other
        var listDestinationLatLng = orders.Select(a => $"{a.Lat},{a.Lng}").ToList();

        // Combine list lat, lng into format to send rest api
        var destinationLatLng = string.Join("|", listDestinationLatLng);

        // Call to Map api
        var oriDistanceRes = await CallServiceGetDistance(originLatLng, destinationLatLng);
        var elements = oriDistanceRes.Rows.First().Elements;

        var distanceOfOrigin = new List<DistanceTraffic>();

        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var element = elements[i];

            var distanceDuration = new DistanceTraffic
            {
                DestinationId = order.Id,
                Distance = element.Distance.Value,
                Duration = element.Duration.Value
            };

            distanceOfOrigin.Add(distanceDuration);
        }

        // Find first order destination closest
        var firstDes = distanceOfOrigin.MinBy(o => useDuration ? o.Duration : o.Distance);
        result.TotalTimeTravel += firstDes.Duration;

        // Get first order des
        var firstOrder = manipulateOrders.FirstOrDefault(o => o.Id == firstDes.DestinationId);

        var firstOrderResponse = _mapper.Map<OrderResponse>(firstOrder);

        var firstResult = new OrderBestRouteResponse
        {
            No = no++,
            Order = firstOrderResponse
        };
        result.ListRoute.Add(firstResult);

        // Delete order already handle in manipulate order list
        manipulateOrders.Remove(firstOrder);

        var nextOrder = firstOrder;
        while (manipulateOrders.Count > 0)
        {
            var minDes = await GetMinDistanceDurationBetweenOrders(nextOrder, manipulateOrders, useDuration);
            result.TotalTimeTravel += minDes.Duration;

            var minOrder = manipulateOrders.FirstOrDefault(o => o.Id == minDes.DestinationId);

            var minOrderResponse = _mapper.Map<OrderResponse>(minOrder);

            var minResult = new OrderBestRouteResponse
            {
                No = no++,
                Order = minOrderResponse
            };

            result.ListRoute.Add(minResult);

            manipulateOrders.Remove(minOrder);
            nextOrder = minOrder;
        }

        //if (manipulateOrders.Count == 1)
        //{
        //    var lastOrder = manipulateOrders.FirstOrDefault();

        //    var lastOrderResponse = _mapper.Map<OrderResponse>(lastOrder);

        //    var lastResult = new OrderBestRouteResponse
        //    {
        //        No = no++,
        //        Order = lastOrderResponse
        //    };

        //    result.ListRoute.Add(lastResult);
        //}

        return result;
    }

    public async Task<OrderBestRouteWrapperResponse> GetMinDurationRandom(
        double originLat,
        double originLng,
        List<OrderInformation> orders)
    {
        var limitSecondRun = _algorithmConfig.MaxSecondRunRandom;
        var limitTimeRun = DateTime.UtcNow.AddSeconds(limitSecondRun);

        var results = new List<OrderBestRouteWrapperResponse>();

        if (orders is null || orders.Count == 0)
        {
            return null;
        }

        var driverLocationTmpId = Guid.NewGuid();
        var cacheDistance = new Dictionary<Guid, List<DistanceTraffic>>();

        var rand = new Random();

        try
        {
            while (DateTime.UtcNow <= limitTimeRun)
            {
                long total = 0;
                var no = 1;
                var listRoute = new List<OrderBestRouteResponse>();

                var manipulateOrder = orders.ToList();

                var randomIndex = rand.Next(0, manipulateOrder.Count);

                var firstOrder = manipulateOrder[randomIndex];

                // First get distance from origin driver lat/lng
                var driverOriginLatLng = $"{originLat},{originLng}";
                var firstOrderDestination = $"{firstOrder.Lat},{firstOrder.Lng}";

                if (cacheDistance.ContainsKey(driverLocationTmpId)
                    && cacheDistance[driverLocationTmpId].FirstOrDefault(d => d.DestinationId == firstOrder.Id) is not null)
                {
                    var listDistance = cacheDistance[driverLocationTmpId];
                    var orderDistance = listDistance.FirstOrDefault(d => d.DestinationId == firstOrder.Id);

                    total += orderDistance.Duration;

                    var des = new OrderBestRouteResponse
                    {
                        No = no++,
                        Order = _mapper.Map<OrderResponse>(firstOrder)
                    };

                    listRoute.Add(des);
                }
                else
                {
                    var distanceFromDriverToFirst = await CallServiceGetDistance(driverOriginLatLng, firstOrderDestination);
                    var elements = distanceFromDriverToFirst.Rows.First().Elements;

                    if (!cacheDistance.ContainsKey(driverLocationTmpId))
                    {
                        cacheDistance.Add(driverLocationTmpId, new List<DistanceTraffic>());
                    }

                    var elementFirst = elements[0];

                    total += elementFirst.Duration.Value;

                    var des = new OrderBestRouteResponse
                    {
                        No = no++,
                        Order = _mapper.Map<OrderResponse>(firstOrder)
                    };

                    listRoute.Add(des);

                    var listCacheDistance = cacheDistance[driverLocationTmpId];
                    listCacheDistance.Add(new DistanceTraffic
                    {
                        DestinationId = firstOrder.Id,
                        Distance = elementFirst.Distance.Value,
                        Duration = elementFirst.Duration.Value
                    });
                }

                manipulateOrder.Remove(firstOrder);
                var nextOrder = firstOrder;

                while (manipulateOrder.Count > 0)
                {
                    var randomOrder = manipulateOrder[rand.Next(0, manipulateOrder.Count)];

                    var origin = $"{nextOrder.Lat},{nextOrder.Lng}";
                    var destination = $"{randomOrder.Lat},{randomOrder.Lng}";

                    if (cacheDistance.ContainsKey(nextOrder.Id)
                        && cacheDistance[nextOrder.Id].FirstOrDefault(d => d.DestinationId == randomOrder.Id) is not null)
                    {
                        var listDistance = cacheDistance[nextOrder.Id];
                        var orderDistance = listDistance.FirstOrDefault(d => d.DestinationId == randomOrder.Id);

                        total += orderDistance.Duration;

                        var des = new OrderBestRouteResponse
                        {
                            No = no++,
                            Order = _mapper.Map<OrderResponse>(randomOrder)
                        };

                        listRoute.Add(des);
                    }
                    else
                    {
                        var nextDistance = await CallServiceGetDistance(origin, destination);
                        var elements = nextDistance.Rows.First().Elements;

                        if (!cacheDistance.ContainsKey(nextOrder.Id))
                        {
                            cacheDistance.Add(nextOrder.Id, new List<DistanceTraffic>());
                        }

                        var elementFirst = elements[0];

                        total += elementFirst.Duration.Value;

                        var des = new OrderBestRouteResponse
                        {
                            No = no++,
                            Order = _mapper.Map<OrderResponse>(randomOrder)
                        };

                        listRoute.Add(des);

                        var listCacheDistance = cacheDistance[nextOrder.Id];
                        listCacheDistance.Add(new DistanceTraffic
                        {
                            DestinationId = randomOrder.Id,
                            Distance = elementFirst.Distance.Value,
                            Duration = elementFirst.Duration.Value
                        });
                    }

                    nextOrder = randomOrder;
                    manipulateOrder.Remove(randomOrder);
                }

                var result = new OrderBestRouteWrapperResponse
                {
                    TotalTimeTravel = total,
                    ListRoute = listRoute
                };
                results.Add(result);
            }
        }
        catch (Exception ex)
        {
            return results.MinBy(a => a.TotalTimeTravel);
        }

        return results.MinBy(a => a.TotalTimeTravel);
    }

    private async Task<DistanceTraffic> GetMinDistanceDurationBetweenOrders(OrderInformation originOrder, List<OrderInformation> orders, bool useDuration)
    {
        var originLatLng = $"{originOrder.Lat},{originOrder.Lng}";

        var listDestinationLatLng = orders.Select(a => $"{a.Lat},{a.Lng}").ToList();

        var destinationLatLng = string.Join("|", listDestinationLatLng);

        var oriDistanceRes = await CallServiceGetDistance(originLatLng, destinationLatLng);
        var elements = oriDistanceRes.Rows.First().Elements;

        var distanceOfOrigin = new List<DistanceTraffic>();

        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var element = elements[i];

            var distanceDuration = new DistanceTraffic
            {
                DestinationId = order.Id,
                Distance = element.Distance.Value,
                Duration = element.Duration.Value
            };

            distanceOfOrigin.Add(distanceDuration);
        }

        var minDes = distanceOfOrigin.MinBy(o => useDuration ? o.Duration : o.Distance);

        return minDes;
    }



    private async Task<DistanceWrapper> CallServiceGetDistance(string origin, string destination)
    {
        var url = new StringBuilder();

        url.Append(_mapConfig.DistanceUrl);
        url.Append('?');
        url.Append($"{ORIGINS_DISTANCE_KEY}={origin}");
        url.Append('&');
        url.Append($"{DESTINATIONS_DISTANCE_KEY}={destination}");
        url.Append('&');
        url.Append($"{VEHICLE_KEY}={VEHICLE_VALUE}");
        url.Append('&');
        url.Append($"{API_KEY}={_mapConfig.ApiKey}");

        var uri = new Uri(url.ToString());

        var distanceWrapper = await _httpClient.GetFromJsonAsync<DistanceWrapper>(uri);

        return distanceWrapper;
    }
}