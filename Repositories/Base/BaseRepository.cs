using System;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using OptimizingLastMile.Configs;

namespace OptimizingLastMile.Repositories.Base;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly DbSet<T> dbSet;
    private readonly OlmDbContext _context;

    public BaseRepository(OlmDbContext context)
    {
        _context = context;
        dbSet = context.Set<T>();
    }

    public virtual async Task<T> GetById<TKey>(TKey id)
    {
        return await dbSet.FindAsync(id);
    }

    public virtual void Create(T type)
    {
        dbSet.Add(type);
    }

    public virtual void Update(T type)
    {
        dbSet.Update(type);
    }

    public virtual void Delete(T type)
    {
        dbSet.Remove(type);
    }

    public virtual async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public IQueryable<T> Get()
    {
        return dbSet;
    }
}

