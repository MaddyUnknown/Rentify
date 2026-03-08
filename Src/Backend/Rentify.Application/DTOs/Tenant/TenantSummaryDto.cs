using Rentify.Application.DTOs.MediaFile;
using Rentify.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Tenant
{
    public class TenantSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public TenantStatusEnum Status { get; set; }
        public MediaFileDto? ProfilePic { get; set; }
    }
}
