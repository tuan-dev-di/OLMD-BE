using System;
namespace OptimizingLastMile.Models.Params;

public class ResourceParam
{
    const int maxPageSize = 20;

    private int _limit = 10;
    public int Limit
    {
        get => _limit;
        set => _limit = (value > maxPageSize) ? maxPageSize : value;
    }
    public int Page { get; set; } = 1;
}

