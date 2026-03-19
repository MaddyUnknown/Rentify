using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Auth.Core.DTOs;
using Rentify.Auth.Core.Exceptions;
using Rentify.Auth.Core.Managers;
using Rentify.Auth.Identity.Entities;
using Rentify.Auth.Identity.Enums;
using Rentify.Auth.Identity.Interfaces;
using Rentify.Auth.Identity.Mappers;
using Rentify.Core.Exceptions;
using System.Security.Authentication;
using InternalEntities = Rentify.Auth.Identity.Entities;

namespace Rentify.Auth.Identity.Managers
{
    public class IdentityAuthManager : IAuthManager
    {
        private UserManager<AppIdentityUser> _userManager;
        private ITokenGenerator _accessTokenGenerator;
        private IRefreshTokenManager _refreshTokenManager;

        public IdentityAuthManager(UserManager<AppIdentityUser> userManager, [FromKeyedServices("auth")] ITokenGenerator accessTokenGenerator, IRefreshTokenManager refreshTokenManager)
        {
            _userManager = userManager;
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenManager = refreshTokenManager;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user == null ? null : UserMapper.MapToUser(user);
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            var userEntity = UserMapper.MapToIdentityUser(user);
            var result = await _userManager.CreateAsync(userEntity, password);

            if (!result.Succeeded) throw new AppValidationException(result.Errors.Select(x => x.Description));

            user.Id = userEntity.Id;
            return user;
        }

        public async Task<UserTokens> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new AppInvalidCredentialException();

            // Check if locked
            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockedTillDateTime = await _userManager.GetLockoutEndDateAsync(user);
                throw new UserLockedOutException(lockedTillDateTime?.LocalDateTime ?? DateTime.Now);
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result)
            {
                await _userManager.AccessFailedAsync(user); // increments failed count
                throw new AppInvalidCredentialException();
            }

            await _userManager.ResetAccessFailedCountAsync(user); // reset lockout count

            var authToken = await _accessTokenGenerator.GenerateTokenAsync(user);
            var refreshToken = await _refreshTokenManager.GenerateNewRefreshToken(user);

            return new UserTokens
            {
                AccessToken = authToken.Token,
                AccessTokenExpiresAt = authToken.ExipryAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExipryAt
            };
        }

        public async Task<UserTokens> RefreshUserTokensAsync(string refreshToken)
        {
            var tokenStatus = await _refreshTokenManager.GetTokenStatusAsync(refreshToken);

            switch(tokenStatus)
            {
                case RefreshTokenStatusEnum.Valid:
                    break;
                case RefreshTokenStatusEnum.Revoked:
                    await _refreshTokenManager.RevokeFamilyAsync(refreshToken);
                    throw new InvalidRefreshTokenException("Revoked token use detected");
                case RefreshTokenStatusEnum.Expired:
                case RefreshTokenStatusEnum.NotFound:
                    throw new InvalidRefreshTokenException();
                default:
                    throw new InvalidRefreshTokenException();
            }

            var user = await _refreshTokenManager.FindUserByTokenAsync(refreshToken);
            if (user == null) throw new InvalidRefreshTokenException();

            var authToken = await _accessTokenGenerator.GenerateTokenAsync(user);
            var newRefreshToken = await _refreshTokenManager.GenerateNewRefreshToken(user, refreshToken);

            return new UserTokens
            {
                AccessToken = authToken.Token,
                AccessTokenExpiresAt = authToken.ExipryAt,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExipryAt
            };
        }
    }
}
