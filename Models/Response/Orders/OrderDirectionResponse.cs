using System;
namespace OptimizingLastMile.Models.Response.Orders;

public class OrderDirectionResponse
{
    public List<GeocodedWaypoint> Geocoded_waypoints { get; set; }
    public List<Route> Routes { get; set; }
}

public class Bounds
{
}

public class Distance
{
    public string Text { get; set; }
    public int Value { get; set; }
}

public class Duration
{
    public string Text { get; set; }
    public int Value { get; set; }
}

public class EndLocation
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class GeocodedWaypoint
{
    public string Geocoder_status { get; set; }
    public string Place_id { get; set; }
}

public class Leg
{
    public Distance Distance { get; set; }
    public Duration Duration { get; set; }
    public string End_address { get; set; }
    public EndLocation End_location { get; set; }
    public string Start_address { get; set; }
    public StartLocation Start_location { get; set; }
    public List<Step> Steps { get; set; }
}

public class OverviewPolyline
{
    public string Points { get; set; }
}

public class Polyline
{
    public string Points { get; set; }
}

public class Route
{
    public Bounds Bounds { get; set; }
    public List<Leg> Legs { get; set; }
    public OverviewPolyline Overview_polyline { get; set; }
    public string Summary { get; set; }
    public List<object> Warnings { get; set; }
    public List<object> Waypoint_order { get; set; }
}

public class StartLocation
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class Step
{
    public Distance Distance { get; set; }
    public Duration Duration { get; set; }
    public EndLocation End_location { get; set; }
    public string Html_instructions { get; set; }
    public string Maneuver { get; set; }
    public Polyline Polyline { get; set; }
    public StartLocation Start_location { get; set; }
    public string Travel_mode { get; set; }
}

