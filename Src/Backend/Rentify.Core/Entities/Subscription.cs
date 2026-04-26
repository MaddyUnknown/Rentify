using Rentify.Core.Abstractions.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rentify.Core.Entities;

public class Subscription : EntityBase
{
    public Subscription() { }

    public Subscription(int id, Guid referenceId, int ownerUserid, DateTime createdDate, DateTime? modifieDate)
    {
        Id = id;
        ReferenceId = referenceId;
        OwnerUserId = ownerUserid;
        CreatedDate = createdDate;
        ModifiedDate = modifieDate;
    }


    [Required]
    public Guid ReferenceId { get; set; }

    [Required]
    public int OwnerUserId { get; set; }
}
