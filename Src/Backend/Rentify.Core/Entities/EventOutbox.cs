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
    public class EventOutbox : EntityBase
    {
        [Required]
        public string EventObjectType { get; set; } = string.Empty;
        
        [Required]
        public string EventData { get; set; } = string.Empty;
    }
}
