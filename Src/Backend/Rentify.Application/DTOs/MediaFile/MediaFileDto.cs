using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.MediaFile
{
    public class MediaFileDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public MediaFileStatusEnum ProcessingStatus { get; set; }

        public MediaFileVariantsDto? Thumbnail { get; set; }
    }
}
