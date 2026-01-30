using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Property
{
    public class CreatePropertyDto
    {
        public CreatePropertyDetailsDto Details { get; set; } = CreatePropertyDetailsDto.Empty;
        public IEnumerable<CreateMediaFileLink> Media { get; set; } = Enumerable.Empty<CreateMediaFileLink>();
        public IEnumerable<CreateUnitDto> Units { get; set; } = Enumerable.Empty<CreateUnitDto>();
        public LocationDto? Location { get; set; }
    }
}
