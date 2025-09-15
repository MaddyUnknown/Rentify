using Rentify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Contract
{
    public int ContractId { get; set; }
    [Required]
    [MaxLength(100)]
    public string ContractNo { get; set; } = string.Empty;
    public int ContractVersion { get; set; } = 1;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public UtilityBillingModeEnum UtilityBillingMode { get; set; } = UtilityBillingModeEnum.None;
    public int RentSettlementDay { get; set; }
    public int? UtilitySettlementDay { get; set; } // Only required for seperate utility billing mode
    public ContractStatusEnum Status { get; set; } = ContractStatusEnum.None;

    // Navigation properties
    public ICollection<ContractUnitAllocation> ContractUnitAllocations { get; set; } = Enumerable.Empty<ContractUnitAllocation>().ToList();
    public ICollection<TenantContractAllocation> TenantContractAllocations { get; set; } = Enumerable.Empty<TenantContractAllocation>().ToList();
}