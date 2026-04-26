using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Enums
{
    public enum RefreshTokenStatusEnum
    {
        Valid,
        Expired,
        Revoked,
        NotFound
    }
}
