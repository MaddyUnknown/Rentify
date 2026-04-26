using Rentify.Core.Abstractions.Entities;
using Rentify.Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Property : EntityBase, ISubscriptionEntity
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
    public int SubscriptionId { get; private set; }
    public int? RequestedCoverPicId { get; set; }
    public int? ActiveCoverPicId { get; set; }

    // Navigation Property
    public Subscription Subscription { get; private set; } = null!;
    public MediaFile? RequestedCoverPic { get; set; } = null;
    public MediaFile? ActiveCoverPic { get; set; } = null;
}
