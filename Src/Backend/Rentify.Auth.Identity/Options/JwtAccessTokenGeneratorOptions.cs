using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Options
{
    public class JwtAccessTokenGeneratorOptions
    {
        public string TokenIssuer { get; set; } = string.Empty;
        public string TokenAudience { get; set; } = string.Empty;
        public string TokenSigningSecret { get; set; } = string.Empty;
        public int TokenDurationInMinutes { get; set; }
    }
}
