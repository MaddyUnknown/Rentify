using Rentify.Core.Abstractions.Entities;
using Rentify.Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Property : EntityBase, IOwnedEntity
{   
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Address PropertyAddress { get; set; } = Address.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public Location? PropertyLocation { get; set; }

    // Foreign Key
    [Required]
    public int OwnerId { get; private set; }

    // Navigation Property
    public Owner Owner { get; private set; } = null!;
}
