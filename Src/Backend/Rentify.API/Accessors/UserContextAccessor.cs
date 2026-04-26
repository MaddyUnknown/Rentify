using Rentify.Auth.Core.Abstractions.Accessors;
using Rentify.Core.Abstractions.Accessors;
using Rentify.Core.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Rentify.API.Accessors
{
    public class UserContextAccessor : IUserContextAccessor
    {
        private static readonly AsyncLocal<DataScopeHolder> _current = new();

        public UserContext UserContext => _current.Value?.Context ?? new();

        public void SetUserContext(int? userId, IEnumerable<Claim> claims)
        {
            _current.Value = new DataScopeHolder
            {
                Context = new(userId, claims)
            };
        }


        private class DataScopeHolder
        {
            public UserContext Context = new();
        }
    }
}
