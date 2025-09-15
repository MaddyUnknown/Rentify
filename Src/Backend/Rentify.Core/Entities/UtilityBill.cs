using Rentify.Core.Enums;

namespace Rentify.Core.Entities;

public class UtilityBill
{
    public int UtilityBillId { get; set; }
    public decimal Amount { get; set; }
    public DateTime BillStartDate { get; set; }
    public DateTime BillEndDate { get; set; }
    public UtilityBillStatusEnum Status { get; set; } = UtilityBillStatusEnum.None;
    
    // Foreign keys
    public int UtilityId { get; set; }

    // Navigation properties
    public Utility Utility { get; set; } = null!;
}
