using Rentify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class ContractRentBreakDown
{
    public int ContractRentBreakDownId { get; set; }
    [Required]
    [MaxLength(100)]
    public string RentItemName { get; set; } = string.Empty;
    public int RentItemLineOrder { get; set; }
    public RentItemTypeEnum RentItemType { get; set; } = RentItemTypeEnum.None;
    public decimal? RentItemAmount { get; set; }
    public decimal? RentItemPercentage { get; set; }
    
    // Foreign keys
    public int TenantContractAllocationId { get; set; }
    public int? UtilityId { get; set; } // nullable for fixed rent items

    // Navigation properties
    public virtual Utility? RentItemUtility { get; set; }
}
