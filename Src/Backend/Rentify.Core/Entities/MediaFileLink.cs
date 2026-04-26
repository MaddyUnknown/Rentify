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
    public class MediaFileLink : EntityBase
    {
        [Required]
        public int EntityId { get; set; }

        [Required]
        public MediaFileEntityEnum EntityType { get; set; }

        // Foreign keys
        [Required]
        public int MediaFileId { get; set; }
    }
}
