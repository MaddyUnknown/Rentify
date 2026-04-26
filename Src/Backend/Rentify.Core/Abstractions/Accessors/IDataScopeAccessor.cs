using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Abstractions.Accessors
{
    public interface IDataScopeAccessor
    {
        DataScope DataScope { get; }
    }
}
