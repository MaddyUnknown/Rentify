using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.DTOs
{
    public class TokenGeneratorResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExipryAt { get; set; }
    }
}
