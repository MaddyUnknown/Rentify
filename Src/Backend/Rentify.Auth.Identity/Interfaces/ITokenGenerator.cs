using Rentify.Auth.Identity.Entities;
using Rentify.Auth.Identity.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Interfaces
{
    public interface ITokenGenerator
    {
        Task<TokenGeneratorResult> GenerateTokenAsync(AppIdentityUser user);
    }
}
