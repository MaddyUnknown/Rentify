using Rentify.Application.Constants;
using Rentify.Application.DTOs.Auth;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Mappers;
using Rentify.Auth.Core.DTOs;
using Rentify.Auth.Core.Managers;
using Rentify.Core.Entities;
using Rentify.Core.Exceptions;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork _unitOfWork;
        private IRepository<Subscription> _subscriptionCRUDRepo;
        private ISubscriptionRepository _subscriptionRepo;
        private IAuthManager _authManager;
        
        public UserService(IUnitOfWork unitOfWork, IRepository<Subscription> subscriptionCRUDRepository, ISubscriptionRepository subscriptionRepository, IAuthManager authManager)
        {
            _unitOfWork = unitOfWork;
            _subscriptionCRUDRepo = subscriptionCRUDRepository;
            _subscriptionRepo = subscriptionRepository;
            _authManager = authManager;
        }

        public async Task<UserDto> RegisterAsync(UserRegisterDto userDto)
        {
            if(userDto.ConfirmPassword != userDto.Password) throw new AppValidationException(UserConstants.ConfirmPasswordNotMatched);

            var user = UserMapper.MapToUser(userDto);
            var registeredUser = await _authManager.RegisterUserAsync(user, userDto.Password);

            //Create subscription
            var subscription = new Subscription { ReferenceId = Guid.NewGuid(), OwnerUserId = user.Id };
            _subscriptionCRUDRepo.Add(subscription);
            await _unitOfWork.SaveChangesAsync();

            return UserMapper.MapToUserDto(registeredUser, subscription);
        }

        public async Task<UserTokens> LoginAsync(UserCredentialsDto userCredentials)
        {
            return await _authManager.LoginAsync(userCredentials.Email, userCredentials.Password);
        }

        public async Task<UserTokens> RefreshUserTokensAsync(string refreshToken)
        {
            return await _authManager.RefreshUserTokensAsync(refreshToken);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _authManager.GetByIdAsync(id);
            if (user == null) throw new AppValidationException(string.Format(UserConstants.UserNotFound, id));

            var subscription = await _subscriptionRepo.GetByUserId(id);
            if(subscription == null) throw new AppValidationException(string.Format(UserConstants.UserNotFound, id));

            return UserMapper.MapToUserDto(user, subscription);
        }
    }
}
