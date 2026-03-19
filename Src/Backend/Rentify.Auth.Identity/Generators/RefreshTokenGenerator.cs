using Rentify.Auth.Identity.Entities;
using Rentify.Auth.Identity.DTOs;
using Rentify.Auth.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Generators
{
    public class RefreshTokenGenerator : ITokenGenerator
    {
        private int _tokenLengthInByte;

        public RefreshTokenGenerator(int tokenLengthInByte)
        {
            _tokenLengthInByte = tokenLengthInByte;
        }

        public Task<TokenGeneratorResult> GenerateTokenAsync(AppIdentityUser user)
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(_tokenLengthInByte);
            return Task.FromResult(new TokenGeneratorResult
            {
                Token = Convert.ToBase64String(tokenBytes),
                ExipryAt = DateTime.MaxValue
            });
        }
    }
}
