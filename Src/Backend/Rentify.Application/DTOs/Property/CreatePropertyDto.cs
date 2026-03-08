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
        public IEnumerable<int> MediaFileIds { get; set; } = Enumerable.Empty<int>();
        public IEnumerable<CreateUnitDto> Units { get; set; } = Enumerable.Empty<CreateUnitDto>();
        public LocationDto? Location { get; set; }
        public int? RequestedCoverPicId { get; set; }
    }
}
