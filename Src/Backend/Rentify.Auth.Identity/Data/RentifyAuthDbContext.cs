using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rentify.Auth.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Data
{
    public class RentifyAuthDbContext : IdentityDbContext<AppIdentityUser, IdentityRole<int>, int>
    {
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        public RentifyAuthDbContext(DbContextOptions<RentifyAuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rename identity tables
            modelBuilder.Entity<AppIdentityUser>().ToTable("Users", "identity");
            modelBuilder.Entity<UserRefreshToken>().ToTable("UserRefreshTokens", "identity");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles", "identity");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles", "identity");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "identity");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", "identity");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", "identity");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", "identity");

        }
    }
}
