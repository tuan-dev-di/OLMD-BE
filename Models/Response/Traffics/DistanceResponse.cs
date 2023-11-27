using System;
namespace OptimizingLastMile.Models.Response.Traffics;

public class DistanceResponse
{
    public AddressResponse From { get; set; }
    public AddressResponse To { get; set; }
    public DistanceDurationResponse Distance { get; set; }
    public DistanceDurationResponse Duration { get; set; }
}

public class AddressResponse
{
    public string Address { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class DistanceDurationResponse
{
    public long Value { get; set; }
    public string Unit { get; set; }
}