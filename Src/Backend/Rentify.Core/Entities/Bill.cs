using Rentify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Bill
{
    public int BillId { get; set; }
    [Required]
    [MaxLength(100)]
    public string BillNumber { get; set; } = string.Empty;
    public int BillVersion { get; set; } = 1;
    public BillTypeEnum BillType { get; set; } = BillTypeEnum.None;
    public DateTime BillStartDate { get; set; }
    public DateTime BillEndDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal AmountPayable { get; set; }
    public decimal AmountPaid { get; set; }
    public BillStatusEnum Status { get; set; } = BillStatusEnum.None;
    
    // Foreign keys
    public int TenantContractAllocationId { get; set; }

    // Navigation propertie
    public ICollection<BillLineItem> BillLineItems { get; set; } = Enumerable.Empty<BillLineItem>().ToList();
    public ICollection<Payment> Payments { get; set; } = Enumerable.Empty<Payment>().ToList();
}
