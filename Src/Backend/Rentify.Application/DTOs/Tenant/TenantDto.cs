using Rentify.Application.DTOs.Location;
using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.DTOs.Property;
using Rentify.Application.DTOs.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Tenant
{
    public class TenantDto
    {
        public int Id { get; set; }
        public GetTenantDetailsDto Details { get; set; } = GetTenantDetailsDto.Empty;
        public TenantEmergencyContactDto EmergencyContact { get; set; } = TenantEmergencyContactDto.Empty;
        public IEnumerable<MediaFileDto> MediaFiles { get; set; } = Enumerable.Empty<MediaFileDto>();
    }
}
