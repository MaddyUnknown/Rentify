using Rentify.Application.DTOs.MediaFile;
using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Property
{
    public class PropertySummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int NumberOfUnits { get; set; }
        public int NumberOfVacantUnits { get; set; }
        public MediaFileDto? CoverImage { get; set; }
    }
}
