using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.MediaFile
{
    public class UpdatePropertyCoverRequestDto
    {
        public int PropertyId { get; set; }
        public int MediaFileId { get; set; }
    }
}
