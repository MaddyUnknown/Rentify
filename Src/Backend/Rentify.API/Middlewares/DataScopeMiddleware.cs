
using Rentify.API.Accessors;
using Rentify.API.Attributes;
using Rentify.API.DTOs;
using Rentify.Application.Interfaces.Services;
using Rentify.Auth.Core.Abstractions.Accessors;
using Rentify.Core.Abstractions.Accessors;
using Rentify.Core.Constants;
using Rentify.Core.Entities;
using Rentify.DataAccess.Core.Repositories;
using System.Text.Json;

namespace Rentify.API.Middlewares
{
    public class DataScopeMiddleware
    {
        public const string SUBSCRIPTION_HEADER = "X-Subscription-Refence";

        private RequestDelegate _next;

        public DataScopeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DataScopeAccessor dataScopeAccessor, IUserContextAccessor userContextAccessor, ISubscriptionService subscriptionService)
        {
            var subscription = context.Request.Headers.ContainsKey(SUBSCRIPTION_HEADER) ? context.Request.Headers[SUBSCRIPTION_HEADER].FirstOrDefault() : string.Empty;
            
            
            if(IsSubscriptionMandatoryInContext(context) && string.IsNullOrEmpty(subscription))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = MediaContentType.Json;
                var json = JsonSerializer.Serialize(ResponseWrapper<object>.ErrorResponse([$"'{SUBSCRIPTION_HEADER}' is mandatory for endpoint"]));
                await context.Response.WriteAsync(json);
            }
            else if(string.IsNullOrEmpty(subscription))
            {
                dataScopeAccessor.SetSubscription(null);
                await _next(context);
            }
            else if(!Guid.TryParse(subscription, out Guid subscriptionReference))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = MediaContentType.Json;
                var json = JsonSerializer.Serialize(ResponseWrapper<object>.ErrorResponse([$"Invalid data for '{SUBSCRIPTION_HEADER}' received. Expect GUID"]));
                await context.Response.WriteAsync(json);
            }
            else
            {
                var subscriptionEntity = await subscriptionService.GetByReferenceId(subscriptionReference);
                if (subscriptionEntity == null) JsonSerializer.Serialize(ResponseWrapper<object>.ErrorResponse([$"Subscription '{subscriptionReference}' not found"]));

                if (!userContextAccessor.UserContext.IsAuthenticated)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = MediaContentType.Json;
                    var json = JsonSerializer.Serialize(ResponseWrapper<object>.ErrorResponse([$"'{SUBSCRIPTION_HEADER}' can be used only when user is authenticated"]));
                    await context.Response.WriteAsync(json);
                }
                else if(subscriptionEntity!.OwnerUserId != userContextAccessor.UserContext.UserId)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = MediaContentType.Json;
                    var json = JsonSerializer.Serialize(ResponseWrapper<object>.ErrorResponse([$"Subscription '{subscriptionReference}' not found"]));
                    await context.Response.WriteAsync(json);
                } else
                {
                    dataScopeAccessor.SetSubscription(subscriptionEntity?.Id);
                    await _next(context);
                }
            }
        }

        private bool IsSubscriptionMandatoryInContext(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var attr = endpoint?.Metadata.GetMetadata<RequireSubscriptionHeaderAttribute>();
            return attr != null;
        }
    }
}
