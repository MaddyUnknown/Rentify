using Rentify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class BillLineItem
{
    public int BillLineItemId { get; set; }
    [Required]
    [MaxLength(100)]
    public string LineItemName { get; set; } = string.Empty;
    public decimal LineItemAmount { get; set; }
    public int LineItemOrder { get; set; }
    public BillItemTypeEnum BillItemType { get; set; } = BillItemTypeEnum.None;
    
    // Foreign keys
    public int BillId { get; set; }
    public int? UtilityBillId { get; set; } // nullable, used for utility-related line items

    // Navigation properties
    public UtilityBill? UtilityBill { get; set; }
}
