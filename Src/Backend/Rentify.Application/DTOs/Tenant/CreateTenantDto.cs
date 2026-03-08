using Rentify.Application.DTOs.MediaFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Tenant
{
    public class CreateTenantDto
    {
        public int? ProfilePicId { get; set; }
        public CreateTenantDetailsDto Details { get; set; } = CreateTenantDetailsDto.Empty;
        public CreateTenantEmergencyContactDto EmergencyContact { get; set; } = CreateTenantEmergencyContactDto.Empty;
        public IEnumerable<int> DocumentIds { get; set; } = Enumerable.Empty<int>();
    }
}
