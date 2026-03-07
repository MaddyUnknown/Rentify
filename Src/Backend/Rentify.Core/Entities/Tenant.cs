using Rentify.Core.Abstractions.Entities;
using Rentify.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Entities
{
    public class Tenant : EntityBase, IOwnedEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime Dob { get; set; }

        [Required]
        [MaxLength(100)]
        public string Employment { get; set; } = string.Empty;

        [Required]
        public Address TenantAddress { get; set; } = Address.Empty;

        [MaxLength(500)]
        public string? Note { get; set; }

        // Foreign Key
        [Required]
        public int OwnerId { get; private set; }
        public int? ProfilePicId { get; set; }

        // Navigation Property
        public Owner Owner { get; private set; } = null!;
        public MediaFile? ProfilePic { get; set; }
    }
}
