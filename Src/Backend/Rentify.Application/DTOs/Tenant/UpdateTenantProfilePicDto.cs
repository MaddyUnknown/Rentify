using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Tenant
{
    public class UpdateTenantProfilePicDto
    {
        public string FileName { get; set; } = string.Empty;
        public Stream MediaStream { get; set; } = Stream.Null;
        public long Length { get; set; }
    }
}
