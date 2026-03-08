using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Contexts
{
    public class MediaProcessingContext
    {
        public MediaFileEntityEnum MediaFileEntity { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public MediaFileVariantEnum Variant { get; set; }
    }
}
