using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Unit
{
    public int UnitId { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    
    // Foreign keys
    public int PropertyId { get; set; }

    // Navigation properties
    public Property Property { get; set; } = null!;
}
