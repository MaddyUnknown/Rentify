using Microsoft.AspNetCore.Identity;
using Rentify.Core.Abstractions.Entities;
using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Entities
{
    public class AppIdentityUser : IdentityUser<int>
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
