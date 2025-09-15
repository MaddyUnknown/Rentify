using Microsoft.EntityFrameworkCore;
using Rentify.Core.Entities;

namespace Rentify.DataAccess.Data;

public class RentifyDbContext : DbContext
{
    public RentifyDbContext(DbContextOptions<RentifyDbContext> options) : base(options) { }

    // DbSets for all entities
    public DbSet<Property> Properties { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Utility> Utilities { get; set; }
    public DbSet<UtilityUnitAllocation> UtilityUnitAllocations { get; set; }
    public DbSet<UtilityBill> UtilityBills { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractUnitAllocation> ContractUnitAllocations { get; set; }
    public DbSet<TenantContractAllocation> TenantContractAllocations { get; set; }
    public DbSet<ContractRentBreakDown> ContractRentBreakDowns { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillLineItem> BillLineItems { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentifyDbContext).Assembly);
    }
}