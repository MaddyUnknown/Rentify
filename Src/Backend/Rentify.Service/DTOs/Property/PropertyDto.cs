using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Property
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public GetPropertyDetailsDto GeneralDetails { get; set; } = GetPropertyDetailsDto.Empty;
        public IEnumerable<MediaFileDto> MediaFiles { get; set; } = Enumerable.Empty<MediaFileDto>();
        public IEnumerable<UnitDto> Units { get; set; } = Enumerable.Empty<UnitDto>();
        public LocationDto? Location { get; set; }

    }
}
