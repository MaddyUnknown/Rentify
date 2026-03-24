using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Core.Abstractions.Accessors
{
    /// <summary>
    /// UserContext - Used as cross cutting data to make user avaliable to application
    /// </summary>
    public class UserContext
    {
        public bool IsAuthenticated => UserId != null;
        public int? UserId { get; private set; }
        public IEnumerable<Claim> Claims { get; private set; }

        public UserContext()
        {
            UserId = null;
            Claims = Enumerable.Empty<Claim>();
        }

        public UserContext(int? userId, IEnumerable<Claim> claims)
        {
            UserId = userId;
            Claims = claims;
        }
    }
}
