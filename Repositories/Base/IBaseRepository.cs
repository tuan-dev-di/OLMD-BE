using System;
using System.Linq.Expressions;

namespace OptimizingLastMile.Repositories.Base;

public interface IBaseRepository<T> where T : class
{
    Task<T> GetById<TKey>(TKey id);
    void Create(T type);
    void Update(T type);
    void Delete(T type);
    Task<int> SaveAsync();
}

