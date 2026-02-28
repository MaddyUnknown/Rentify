using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.MediaFile
{
    public class UploadMediaFileDto
    {
        public MediaFileEntityEnum EntityType { get; set; }
        public int? EntityId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public Stream MediaStream { get; set; } = Stream.Null;
        public long Length { get; set; }
        public IEnumerable<MediaFileUsageEnum> Usage { get; set; } = Enumerable.Empty<MediaFileUsageEnum>();
    }
}
