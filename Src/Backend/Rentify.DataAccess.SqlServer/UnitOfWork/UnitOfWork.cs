using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Rentify.Core.Abstractions.Entities;
using Rentify.Core.Exceptions;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.DataAccess.SqlServer.Constants;
using Rentify.DataAccess.SqlServer.Data;

namespace Rentify.DataAccess.SqlServer.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly RentifyDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(RentifyDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null) throw new InvalidOperationException(DataAccessConstant.TransactionInProgressStarted);

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null) throw new InvalidOperationException(DataAccessConstant.TransactionNotActive);

        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null) throw new InvalidOperationException(DataAccessConstant.TransactionInProgressStarted);

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
