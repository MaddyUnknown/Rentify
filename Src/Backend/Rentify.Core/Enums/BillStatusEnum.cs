using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Enums
{
    public enum BillStatusEnum
    {
        None = 0,
        InReview = 1,
        Generated = 2,
        Cancelled = 3
    }
}
