using Rentify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; } = PaymentMethodEnum.None;
    [Required]
    [MaxLength(200)]
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public PaymentStatusEnum Status { get; set; } = PaymentStatusEnum.None;
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    // Foreign keys
    public int BillId { get; set; }
}
