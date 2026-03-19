using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Constants
{
    public static class AuthConstants
    {
        public static readonly string ConnectionStringNotConfigurated = "Connection string not configured for auth data access";
        public static readonly string JwtIssuerNotConfigurated = "Jwt token issuer string not configured for auth data access";
        public static readonly string JwtAudienceNotConfigurated = "Jwt token audience string not configured for auth data access";
        public static readonly string JwtSigningSecretNotConfigurated = "Jwt token signing secret not configured for auth data access";
    }
}
