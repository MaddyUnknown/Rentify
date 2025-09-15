namespace Rentify.Core.Entities;

public class TenantContractAllocation
{
    public int TenantContractAllocationId { get; set; }
    
    // Foreign keys
    public int ContractId { get; set; }
    public int TenantId { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public ICollection<ContractRentBreakDown> ContractRentBreakDowns { get; set; } = Enumerable.Empty<ContractRentBreakDown>().ToList();
    public ICollection<Bill> Bills { get; set; } = Enumerable.Empty<Bill>().ToList();
    public ICollection<Deposit> Deposits { get; set; } = Enumerable.Empty<Deposit>().ToList();
}
