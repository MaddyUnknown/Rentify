using Rentify.Core.Abstractions.Entities;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Entities
{
    public class MediaFile: EntityBase, ISubscriptionEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public string FileKey { get; set; } = string.Empty;

        [Required]
        public long Length { get; set; } = 0;

        [Required]
        public MediaFileStatusEnum Status { get; set; }

        // Foreign keys
        [Required]
        public int SubscriptionId { get; private set; }

        // Navigation properties
        public Subscription Subscription { get; private set; } = null!;

        public ICollection<MediaFileVariant> MediaFileVariants { get; set; } = new HashSet<MediaFileVariant>();
        
        public MediaFileLink MediaFileLink { get; set; } = null!;
    }
}
