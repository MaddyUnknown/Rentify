using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Location
{
    public class UpdatePropertyLocationDto
    {
        public decimal? Latitute { get; set; }
        public decimal? Longitute { get; set; }
    }
}
