using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Location
{
    public class UpdatePropertyLocationDto
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
