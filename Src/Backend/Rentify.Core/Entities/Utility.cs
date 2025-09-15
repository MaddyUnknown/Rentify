using Rentify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Utility
{
    public int UtilityId { get; set; }
    public UtilityTypeEnum UtilityType { get; set; } = UtilityTypeEnum.None;
    [Required]
    [MaxLength(100)]
    public string ProviderName { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string UtilityExternalId { get; set; } = string.Empty;
    public BillingCycleEnum BillingCycle { get; set; } = BillingCycleEnum.None;
    public DateTime BillingCycleStartDate { get; set; }
    
    // Foreign keys
    public int PropertyId { get; set; }

    // Navigation properties
    public Property Property { get; set; } = null!;
    public ICollection<UtilityUnitAllocation> UtilityUnitAllocations { get; set; } = Enumerable.Empty<UtilityUnitAllocation>().ToList();
}
