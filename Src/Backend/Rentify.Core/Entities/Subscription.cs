using Rentify.Core.Abstractions.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Subscription : EntityBase
{
    [Required]
    public Guid ReferenceId { get; set; }

    [Required]
    public int OwnerUserId { get; set; }
}
