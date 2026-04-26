using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using Rentify.Auth.Identity.Entities;
using Rentify.Auth.Identity.DTOs;
using Rentify.Auth.Identity.Interfaces;
using Rentify.Auth.Identity.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Generators
{
    public class JwtAccessTokenGenerator : ITokenGenerator
    {
        private JwtAccessTokenGeneratorOptions _options;

        public JwtAccessTokenGenerator(JwtAccessTokenGeneratorOptions options)
        {
            _options = options;
        }

        public Task<TokenGeneratorResult> GenerateTokenAsync(AppIdentityUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            var signingKeyByte = Encoding.UTF8.GetBytes(_options.TokenSigningSecret);
            var signingKey = new SymmetricSecurityKey(signingKeyByte);

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var tokenExpiry = DateTime.Now.AddMinutes(_options.TokenDurationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _options.TokenIssuer,
                audience: _options.TokenAudience,
                claims: claims,
                expires: tokenExpiry,
                signingCredentials: creds
            );

            return Task.FromResult(new TokenGeneratorResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExipryAt = tokenExpiry
            });
        }
    }
}
