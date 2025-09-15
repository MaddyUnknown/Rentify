using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rentify.DataAccess.Data;

namespace Rentify.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly RentifyDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(RentifyDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, params string[] includeProperties)
    {
        IQueryable<T> query = _dbSet;

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty.Trim());
            }
        }

        return await query.FirstOrDefaultAsync(filter);
    }

    public async Task<IEnumerable<T>> GetAllAsync(params string[] includeProperties)
    {
        IQueryable<T> query = _dbSet;

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty.Trim());
            }
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> filter, params string[] includeProperties)
    {
        IQueryable<T> query = _dbSet;

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty.Trim());
            }
        }

        return await query.Where(filter).ToListAsync();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
    {
        if (filter == null)
        {
            return await _dbSet.CountAsync();
        }

        return await _dbSet.CountAsync(filter);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbSet.AnyAsync(filter);
    }
}
