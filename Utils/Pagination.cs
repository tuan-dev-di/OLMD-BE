using System;
using Microsoft.EntityFrameworkCore;

namespace OptimizingLastMile.Utils;

public class Pagination<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public List<T> Data { get; private set; }
    public bool HasPrevious => (CurrentPage > 1);
    public bool HasNext => (CurrentPage < TotalPage);

    public Pagination(List<T> data, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPage = (int)Math.Ceiling(count / (double)pageSize);
        Data = data;
    }

    public static async Task<Pagination<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var data = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new Pagination<T>(data, count, pageNumber, pageSize);
    }
}