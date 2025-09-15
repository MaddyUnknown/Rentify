using Rentify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Deposit
{
    public int DepositId { get; set; }
    [Required]
    [MaxLength(100)]
    public string DepositName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal RefundedAmount { get; set; }
    public DepositStatusEnum Status { get; set; } = DepositStatusEnum.None;

    // Foreign keys
    public int TenantContractAllocationId { get; set; }
}
