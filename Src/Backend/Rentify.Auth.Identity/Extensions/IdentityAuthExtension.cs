using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Rentify.Auth.Core.Managers;
using Rentify.Auth.Identity.Constants;
using Rentify.Auth.Identity.Data;
using Rentify.Auth.Identity.Entities;
using Rentify.Auth.Identity.Generators;
using Rentify.Auth.Identity.Interfaces;
using Rentify.Auth.Identity.Managers;
using Rentify.Auth.Identity.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Rentify.Auth.Identity.Extensions
{
    public static class IdentityAuthExtension
    {
        public static void AddIdentityAuthServices(this IServiceCollection serviceCollection, Action<IdentitySetupOptions> optionsSetupAction)
        {
            IdentitySetupOptions authOptions = new IdentitySetupOptions();
            optionsSetupAction(authOptions);

            if(authOptions.ConnectionString == null) throw new ArgumentNullException(nameof(authOptions.ConnectionString), AuthConstants.ConnectionStringNotConfigurated);
            if (authOptions.JwtTokenIssuer == null) throw new ArgumentNullException(nameof(authOptions.JwtTokenIssuer), AuthConstants.JwtIssuerNotConfigurated);
            if (authOptions.JwtTokenAudience == null) throw new ArgumentNullException(nameof(authOptions.JwtTokenAudience), AuthConstants.JwtAudienceNotConfigurated);
            if (authOptions.JwtAccessTokenSigningSecret == null) throw new ArgumentNullException(nameof(authOptions.JwtAccessTokenSigningSecret), AuthConstants.JwtSigningSecretNotConfigurated);

            authOptions.JwtAccessTokenDurationInMinutes ??= 15; // 15 min by default
            authOptions.JwtRefreshTokenDurationInDays ??= 7; // 7 day by default

            // Add Rentify DbContext
            serviceCollection.AddDbContext<RentifyAuthDbContext>(options =>
            {
                options.UseSqlServer(authOptions.ConnectionString);
            });

            serviceCollection.AddIdentityCore<AppIdentityUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 2;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<RentifyAuthDbContext>();


            var signingKey = Encoding.UTF8.GetBytes(authOptions.JwtAccessTokenSigningSecret);

            //Add authentication
            serviceCollection
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidIssuer = authOptions.JwtTokenIssuer,
                    ValidAudience = authOptions.JwtTokenAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKey)
                };
            });

            // Add generators
            serviceCollection.AddKeyedTransient<ITokenGenerator>("auth", (provider, key) =>
            {
                return new JwtAccessTokenGenerator(new JwtAccessTokenGeneratorOptions
                {
                    TokenAudience = authOptions.JwtTokenAudience,
                    TokenIssuer = authOptions.JwtTokenIssuer,
                    TokenSigningSecret = authOptions.JwtAccessTokenSigningSecret,
                    TokenDurationInMinutes = authOptions.JwtAccessTokenDurationInMinutes.Value
                });
            });
            serviceCollection.AddKeyedTransient<ITokenGenerator, RefreshTokenGenerator>("refresh", (provider, key) =>
            {
                return new RefreshTokenGenerator(32);
            });

            // Add managers
            serviceCollection.AddTransient<IAuthManager, IdentityAuthManager>();
            serviceCollection.AddTransient<IRefreshTokenManager, RefreshTokenManager>(provider =>
            {
                var refreshTokenGenerator = provider.GetRequiredKeyedService<ITokenGenerator>("refresh");
                var dbContext = provider.GetRequiredService<RentifyAuthDbContext>();
                var tokenDurationInDays = authOptions.JwtRefreshTokenDurationInDays.Value;

                return new RefreshTokenManager(refreshTokenGenerator, dbContext, tokenDurationInDays);
            });
        }
    }
}
