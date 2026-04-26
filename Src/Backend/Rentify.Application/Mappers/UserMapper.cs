using Rentify.Application.DTOs.Auth;
using Rentify.Auth.Core.DTOs;
using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Mappers
{
    public static class UserMapper
    {
        public static UserDto MapToUserDto(User user, Subscription subscription)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                SubscriptionReferenceId = subscription.ReferenceId.ToString()
            };
        } 

        public static User MapToUser(UserRegisterDto user)
        {
            return new User
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }
    }
}
