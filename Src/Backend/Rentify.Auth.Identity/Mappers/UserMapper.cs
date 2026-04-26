using Rentify.Auth.Core.DTOs;
using Rentify.Auth.Identity.Entities;
using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternalEntities = Rentify.Auth.Identity.Entities;

namespace Rentify.Auth.Identity.Mappers
{
    public static class UserMapper
    {
        public static User MapToUser(AppIdentityUser user)
        {
            return new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
            };
        }

        public static AppIdentityUser MapToIdentityUser(User user)
        {
            return new AppIdentityUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }
    }
}
