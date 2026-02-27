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
        public CreateTenantDetailsDto Details { get; set; } = CreateTenantDetailsDto.Empty;
        public CreateTenantEmergencyContactDto EmergencyContact { get; set; } = CreateTenantEmergencyContactDto.Empty;
        public IEnumerable<CreateMediaFileLink> Documents { get; set; } = Enumerable.Empty<CreateMediaFileLink>();
    }
}
