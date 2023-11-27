using System;
namespace OptimizingLastMile.Entites;

public class Device
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; }
    public long AccountId { get; set; }
    public DateTime CreatedDate { get; set; }
}

