using Rentify.Core.Entities;
using Rentify.DataAccess.Repositories;

namespace Rentify.DataAccess.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<Property> Properties { get; }
    IRepository<Unit> Units { get; }
    IRepository<Utility> Utilities { get; }
    IRepository<UtilityUnitAllocation> UtilityUnitAllocations { get; }
    IRepository<UtilityBill> UtilityBills { get; }
    IRepository<Tenant> Tenants { get; }
    IRepository<Contract> Contracts { get; }
    IRepository<ContractUnitAllocation> ContractUnitAllocations { get; }
    IRepository<TenantContractAllocation> TenantContractAllocations { get; }
    IRepository<ContractRentBreakDown> ContractRentBreakDowns { get; }
    IRepository<Deposit> Deposits { get; }
    IRepository<Bill> Bills { get; }
    IRepository<BillLineItem> BillLineItems { get; }
    IRepository<Payment> Payments { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
