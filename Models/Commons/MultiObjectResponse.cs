using System;
namespace OptimizingLastMile.Models.Commons;

public class MultiObjectResponse<T>
{
    public int CurrentPage { get; set; }
    public int TotalPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<T> Data { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}