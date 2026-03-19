using Rentify.Auth.Core.DTOs;
using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Core.Managers
{
    public interface IAuthManager
    {
        public Task<User?> GetByIdAsync(int id);
        public Task<User> RegisterUserAsync(User user, string password);
        public Task<UserTokens> LoginAsync(string email, string password);
        public Task<UserTokens> RefreshUserTokensAsync(string refreshToken);
    }
}
