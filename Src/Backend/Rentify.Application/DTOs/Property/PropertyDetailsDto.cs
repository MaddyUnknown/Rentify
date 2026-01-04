using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Property
{
    public class PropertyDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StreetName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public static PropertyDetailsDto Empty => new PropertyDetailsDto();

    }
}
