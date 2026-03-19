using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Options
{
    public class IdentitySetupOptions
    {
        public string? ConnectionString { get; set; }
        public string? JwtTokenIssuer { get; set; }
        public string? JwtTokenAudience { get; set; }
        public string? JwtAccessTokenSigningSecret { get; set; }
        public int? JwtAccessTokenDurationInMinutes { get; set; }
        public int? JwtRefreshTokenDurationInDays { get; set; }
    }
}
