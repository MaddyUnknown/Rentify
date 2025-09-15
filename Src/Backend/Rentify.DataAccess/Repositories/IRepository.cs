using System.Linq.Expressions;

namespace Rentify.DataAccess.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, params string[] includeProperties);
    Task<IEnumerable<T>> GetAllAsync(params string[] includeProperties);
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> filter, params string[] includeProperties);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
}