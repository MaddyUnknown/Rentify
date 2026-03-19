using Rentify.Auth.Identity.Entities;
using Rentify.Auth.Identity.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentify.Auth.Identity.Enums;

namespace Rentify.Auth.Identity.Interfaces
{
    public interface IRefreshTokenManager
    {
        Task<TokenGeneratorResult> GenerateNewRefreshToken(AppIdentityUser user, string? oldTokenStr = null);
        Task<RefreshTokenStatusEnum> GetTokenStatusAsync(string tokenStr);
        Task RevokeFamilyAsync(string tokenStr);
        Task<AppIdentityUser?> FindUserByTokenAsync(string tokenStr);
    }
}
