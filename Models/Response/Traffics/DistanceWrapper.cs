using System;
namespace OptimizingLastMile.Models.Response.Traffics;

public class DistanceWrapper
{
    public List<DistanceRow> Rows { get; set; }
}

public class DistanceRow
{
    public List<DistanceRowElement> Elements { get; set; }
}

public class DistanceRowElement
{
    public string Status { get; set; }
    public Duration Duration { get; set; }
    public Distance Distance { get; set; }

}

public class Duration
{
    public string Text { get; set; }
    public long Value { get; set; }
}

public class Distance
{
    public string Text { get; set; }
    public long Value { get; set; }
}