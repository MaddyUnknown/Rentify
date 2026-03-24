
using Rentify.API.Accessors;
using Rentify.API.DTOs;
using Rentify.Core.Abstractions.Accessors;
using Rentify.Core.Constants;
using Rentify.Core.Entities;
using Rentify.DataAccess.Core.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Rentify.API.Middlewares
{
    public class UserContextMiddleware
    {
        private RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserContextAccessor userContextAccessor, IHttpContextAccessor httpContextAccessor)
        {

            if (httpContextAccessor == null || !(httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false))
            {
                userContextAccessor.SetUserContext(null, []);
                await _next(context);
                return;
            }

            var claims = httpContextAccessor.HttpContext.User.Claims;
            var subClaim = httpContextAccessor.HttpContext.User.FindFirstValue("sub");
            if (!int.TryParse(subClaim, out int subClaimId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = MediaContentType.Json;
                var json = JsonSerializer.Serialize(ResponseWrapper<object>.ErrorResponse([$"'sub' not present in user claims"]));
                await context.Response.WriteAsync(json);
                return;
            }

            userContextAccessor.SetUserContext(subClaimId, claims);
            await _next(context);
        }
    }
}
