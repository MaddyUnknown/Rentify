using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.MediaFile
{
    public class PropertySearchDto
    {
        public int CurrentPage { get; set; }
        public int TotalItemPerPage { get; set; }
        public DateTime AsOfDate { get; set; }
    }
}
