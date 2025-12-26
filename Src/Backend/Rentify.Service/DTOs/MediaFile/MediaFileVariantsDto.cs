using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.MediaFile
{
    public class MediaFileVariantsDto
    {
        public string ContentType { get; set; } = string.Empty;
        public MediaFileVariantStatusEnum ProcessingStatus { get; set; }
    }
}
