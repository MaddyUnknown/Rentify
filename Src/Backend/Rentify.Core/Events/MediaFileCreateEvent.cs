using Rentify.Core.Abstractions.Events;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Events
{
    public class MediaFileCreateEvent : EventBase
    {
        public int MediaFileId { get; set; }
        public MediaFileEntityEnum MediaFileEntity { get; set; }
        public MediaFileVariantEnum[] Variants { get; set; } = Array.Empty<MediaFileVariantEnum>();
    }
}
