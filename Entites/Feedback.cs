using System;
namespace OptimizingLastMile.Entites;

public class Feedback
{
    public Guid Id { get; set; }
    public double Rate { get; set; }
    public string Comment { get; set; }

    public Guid OrderId { get; set; }

    public long CustomerId { get; set; }
    public long DriverId { get; set; }
}

