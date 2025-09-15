namespace Rentify.Core.Entities;

public class ContractUnitAllocation
{
    public int ContractUnitAllocationId { get; set; }
    
    // Foreign keys
    public int ContractId { get; set; }
    public int UnitId { get; set; }

    // Navigation properties
    public virtual Unit Unit { get; set; } = null!;
}
