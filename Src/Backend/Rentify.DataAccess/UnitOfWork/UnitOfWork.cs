using Microsoft.EntityFrameworkCore.Storage;
using Rentify.Core.Entities;
using Rentify.DataAccess.Data;
using Rentify.DataAccess.Repositories;

namespace Rentify.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly RentifyDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(RentifyDbContext context)
    {
        _context = context;
        Properties = new Repository<Property>(_context);
        Units = new Repository<Unit>(_context);
        Utilities = new Repository<Utility>(_context);
        UtilityUnitAllocations = new Repository<UtilityUnitAllocation>(_context);
        UtilityBills = new Repository<UtilityBill>(_context);
        Tenants = new Repository<Tenant>(_context);
        Contracts = new Repository<Contract>(_context);
        ContractUnitAllocations = new Repository<ContractUnitAllocation>(_context);
        TenantContractAllocations = new Repository<TenantContractAllocation>(_context);
        ContractRentBreakDowns = new Repository<ContractRentBreakDown>(_context);
        Deposits = new Repository<Deposit>(_context);
        Bills = new Repository<Bill>(_context);
        BillLineItems = new Repository<BillLineItem>(_context);
        Payments = new Repository<Payment>(_context);
    }

    public IRepository<Property> Properties { get; }
    public IRepository<Unit> Units { get; }
    public IRepository<Utility> Utilities { get; }
    public IRepository<UtilityUnitAllocation> UtilityUnitAllocations { get; }
    public IRepository<UtilityBill> UtilityBills { get; }
    public IRepository<Tenant> Tenants { get; }
    public IRepository<Contract> Contracts { get; }
    public IRepository<ContractUnitAllocation> ContractUnitAllocations { get; }
    public IRepository<TenantContractAllocation> TenantContractAllocations { get; }
    public IRepository<ContractRentBreakDown> ContractRentBreakDowns { get; }
    public IRepository<Deposit> Deposits { get; }
    public IRepository<Bill> Bills { get; }
    public IRepository<BillLineItem> BillLineItems { get; }
    public IRepository<Payment> Payments { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
