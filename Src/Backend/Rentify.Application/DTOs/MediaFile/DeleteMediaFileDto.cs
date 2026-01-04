using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.DTOs.MediaFile
{
    public class DeleteMediaFileDto
    {
        public int Id { get; set; }
        public MediaFileEntityEnum EntityType { get; set; }
        public int EntityId { get; set; }
    }
}
