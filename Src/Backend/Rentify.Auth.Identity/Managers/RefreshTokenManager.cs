using Microsoft.Extensions.DependencyInjection;
using Rentify.Auth.Core.DTOs;
using Rentify.Auth.Identity.Data;
using Rentify.Auth.Identity.DTOs;
using Rentify.Auth.Identity.Entities;
using Rentify.Auth.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Rentify.Auth.Identity.Enums;
using Microsoft.EntityFrameworkCore;

namespace Rentify.Auth.Identity.Managers
{
    public class RefreshTokenManager : IRefreshTokenManager
    {
        private ITokenGenerator _refreshTokenGenerator;
        private RentifyAuthDbContext _dbContext;
        private int _tokenDurationInDays;

        public RefreshTokenManager([FromKeyedServices("refresh")] ITokenGenerator refreshTokenGenerator, RentifyAuthDbContext dbContext, int tokenDurationInDays)
        {
            _refreshTokenGenerator = refreshTokenGenerator;
            _dbContext = dbContext;
            _tokenDurationInDays = tokenDurationInDays;
        }

        public async Task<TokenGeneratorResult> GenerateNewRefreshToken(AppIdentityUser user, string? oldTokenStr = null)
        {
            var newRefreshToken = await _refreshTokenGenerator.GenerateTokenAsync(user);
            var hashedToken = GenerateHashForToken(newRefreshToken.Token);

            var entity = new UserRefreshToken
            {
                UserId = user.Id,
                TokenHash = hashedToken,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(_tokenDurationInDays),
            };

            _dbContext.UserRefreshTokens.Add(entity);

            if (oldTokenStr != null)
            {
                var oldHashedToken = GenerateHashForToken(oldTokenStr);
                var oldTokenEntity = await _dbContext.UserRefreshTokens.Where(t => t.TokenHash == oldHashedToken).FirstOrDefaultAsync();
                
                if(oldTokenEntity != null)
                {
                    oldTokenEntity.RevokedAt = DateTime.Now;
                    oldTokenEntity.ReplacedByToken = entity;
                }
            }

            await _dbContext.SaveChangesAsync();


            return new TokenGeneratorResult
            {
                Token = newRefreshToken.Token,
                ExipryAt = entity.ExpiresAt,
            };
        }

        public async Task<RefreshTokenStatusEnum> GetTokenStatusAsync(string tokenStr)
        {
            var hashedToken = GenerateHashForToken(tokenStr);
            var tokenEntity = await _dbContext.UserRefreshTokens.Where(t => t.TokenHash == hashedToken).FirstOrDefaultAsync();

            if (tokenEntity == null)
            {
                return RefreshTokenStatusEnum.NotFound;
            }
            else if(tokenEntity.RevokedAt != null)
            {
                return RefreshTokenStatusEnum.Revoked;
            } 
            else if (tokenEntity.ExpiresAt <= DateTime.Now)
            {
                return RefreshTokenStatusEnum.Expired;
            }
            else
            {
                return RefreshTokenStatusEnum.Valid;
            }
        }

        public async Task RevokeFamilyAsync(string tokenStr)
        {
            var hashedToken = GenerateHashForToken(tokenStr);
            var tokenEntity = await _dbContext.UserRefreshTokens.Where(t => t.TokenHash == hashedToken).FirstOrDefaultAsync();

            while (tokenEntity != null)
            {
                tokenEntity.RevokedAt = DateTime.Now;

                tokenEntity = tokenEntity.ReplacedByTokenId.HasValue
                    ? await _dbContext.UserRefreshTokens.Where(t => t.Id == tokenEntity.ReplacedByTokenId.Value).FirstOrDefaultAsync()
                    : null;
            }
            

            await _dbContext.SaveChangesAsync();
        }

        public async Task<AppIdentityUser?> FindUserByTokenAsync(string tokenStr)
        {
            var hashedToken = GenerateHashForToken(tokenStr);
            var tokenEntity = await _dbContext.UserRefreshTokens.Include(t => t.User).Where(t => t.TokenHash == hashedToken).FirstOrDefaultAsync();

            return tokenEntity?.User;
        }

        private string GenerateHashForToken(string token) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
