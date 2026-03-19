using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Entities
{
    public class UserRefreshToken
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string TokenHash { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }


        // Foreign Key
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(ReplacedByToken))]
        public int? ReplacedByTokenId { get; set; }


        // Navigation Property
        public AppIdentityUser User { get; set; } = null!;

        public UserRefreshToken? ReplacedByToken { get; set; }
    }
}
