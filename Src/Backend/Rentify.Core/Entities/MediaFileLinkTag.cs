using Rentify.Core.Abstractions.Entities;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Entities
{
    public class MediaFileLinkTag : EntityBase
    {
        public MediaFileLinkTagEnum Tag { get; set; }
    }
}
