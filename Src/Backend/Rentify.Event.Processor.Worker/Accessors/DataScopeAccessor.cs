using Rentify.Core.Abstractions.Accessors;
using Rentify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Processor.Worker.Accessors
{
    public class DataScopeAccessor : IDataScopeAccessor
    {
        private DataScope _scope = new(DataScopeModeEnum.FullAccess);
        public DataScope DataScope => _scope;
    }
}
