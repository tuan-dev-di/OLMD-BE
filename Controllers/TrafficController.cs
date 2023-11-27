using System;
using Microsoft.AspNetCore.Mvc;
using OptimizingLastMile.Models.Requests.Traffics;
using OptimizingLastMile.Services.Maps;

namespace OptimizingLastMile.Controllers;

[ApiController]
[Route("api/traffics")]
public class TrafficController : ControllerBase
{
    private readonly IMapService _mapService;

    public TrafficController(IMapService mapService)
    {
        this._mapService = mapService;
    }

    [HttpPost]
    public async Task<IActionResult> GetDistance([FromBody] List<DistanceRequestPayload> payloads)
    {
        var distanceResponses = await _mapService.GetDistance(payloads);
        return Ok(distanceResponses);
    }
}

