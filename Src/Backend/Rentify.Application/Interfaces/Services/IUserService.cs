using Rentify.Application.DTOs.Auth;
using Rentify.Auth.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(UserRegisterDto userDto);
        Task<UserTokens> LoginAsync(UserCredentialsDto userCredentials);
        Task<UserTokens> RefreshUserTokensAsync(string refreshToken);
    }
}
