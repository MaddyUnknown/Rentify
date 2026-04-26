using Rentify.Core.Abstractions.Accessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Core.Abstractions.Accessors
{
    public interface IUserContextAccessor
    {
        UserContext UserContext { get; }
    }
}
