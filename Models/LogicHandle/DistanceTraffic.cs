using System;
namespace OptimizingLastMile.Models.LogicHandle;

public class DistanceTraffic
{
    public Guid DestinationId { get; set; }
    public long Distance { get; set; }
    public long Duration { get; set; }
}