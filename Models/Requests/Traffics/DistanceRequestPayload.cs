using System;
namespace OptimizingLastMile.Models.Requests.Traffics;

public class DistanceRequestPayload
{
    public string Address { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
}