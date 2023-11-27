using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Requests.Drivers;

public class DriverStatusUpdatePayload
{
    public StatusEnum Status { get; set; }
}