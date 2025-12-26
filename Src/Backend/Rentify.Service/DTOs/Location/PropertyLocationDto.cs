using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Location
{
    public class PropertyLocationDto
    {
        public int PropertyId { get; set; }
        public decimal? Latitute { get; set; }
        public decimal? Longitute { get; set; }
    }
}
