using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Enums
{
    public enum MediaFileStatusEnum
    {
        Unknown = 0,
        Uploaded = 1,
        Processing = 2,
        Processed = 3,
        Failed = 4,
        Deleted = 5
    }
}
