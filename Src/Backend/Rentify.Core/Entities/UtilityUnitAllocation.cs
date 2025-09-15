namespace Rentify.Core.Entities;

public class UtilityUnitAllocation
{
    public int UtilityUnitAllocationId { get; set; }
    
    // Foreign keys
    public int UtilityId { get; set; }
    public int UnitId { get; set; }

    // Navigation properties
    public Unit Unit { get; set; } = null!;
}