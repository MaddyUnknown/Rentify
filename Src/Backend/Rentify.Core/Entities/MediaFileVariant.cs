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
    public class MediaFileVariant : EntityBase
    {
        [Required]
        public MediaFileVariantEnum VariantType { get; set; }

        [Required]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public string FileKey { get; set; } = string.Empty;

        [Required]
        public MediaFileVariantStatusEnum Status { get; set; }

        // Foreign keys
        [Required]
        public int MediaFileId { get; set; }
    }
}
