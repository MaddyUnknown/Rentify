using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Tenant
{
    public int TenantId { get; set; }
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MaxLength(500)]
    public string PermanentAddress { get; set; } = string.Empty;
}
