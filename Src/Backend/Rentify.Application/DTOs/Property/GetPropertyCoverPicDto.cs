using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.Property
{
    public class GetPropertyCoverPicDto
    {
        public int? RequestedCoverPicId { get; set; }
        public int? ActiveCoverPicId { get; set; }

        public static GetPropertyCoverPicDto Empty => new GetPropertyCoverPicDto();
    }
}
