using Rentify.Core.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Unit : EntityBase, ISubscriptionEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public int Size { get; set; }

    // Foreign keys
    [Required]
    public int PropertyId { get; set; }

    [Required]
    public int SubscriptionId { get; private set; }

    // Navigation properties
    public Property Property { get; set; } = null!;

    public Subscription Subscription { get; private set; } = null!;
}
