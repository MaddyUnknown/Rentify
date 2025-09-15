using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Property
{
    public int PropertyId { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
}
