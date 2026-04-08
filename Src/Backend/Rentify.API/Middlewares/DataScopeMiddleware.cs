
using Microsoft.Extensions.Options;
using Rentify.API.Accessors;
using Rentify.API.Attributes;
using Rentify.API.DTOs;
using Rentify.Application.Interfaces.Services;
using Rentify.Auth.Core.Abstractions.Accessors;
using Rentify.Core.Abstractions.Accessors;
using Rentify.Core.Constants;
using Rentify.Core.Entities;
using Rentify.Core.Utils;
using Rentify.DataAccess.Core.Repositories;
using System.Text.Json;

namespace Rentify.API.Middlewares
{
    public class DataScopeMiddleware
    {
        public const string SUBSCRIPTION_HEADER = "X-Subscription-Refence";

        private RequestDelegate _next;
        private JsonSerializerOptions _responseSerializerOption;

        public DataScopeMiddleware(RequestDelegate next, IOptionsMonitor<JsonSerializerOptions> optionsMonitor)
        {
            _next = next;
            _responseSerializerOption = optionsMonitor.Get("response-serializer-option");
        }

        public async Task InvokeAsync(HttpContext context, DataScopeAccessor dataScopeAccessor, IUserContextAccessor userContextAccessor, ISubscriptionService subscriptionService)
        {
            var subscription = context.Request.Headers.ContainsKey(SUBSCRIPTION_HEADER) ? context.Request.Headers[SUBSCRIPTION_HEADER].FirstOrDefault() : string.Empty;
            
            
            if(IsSubscriptionMandatoryInContext(context) && string.IsNullOrEmpty(subscription))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = MediaContentType.Json;
                var json = JsonSerializerHelper.Serialize(ResponseWrapper<object>.ErrorResponse([$"'{SUBSCRIPTION_HEADER}' is mandatory for endpoint"]), _responseSerializerOption);
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
                var json = JsonSerializerHelper.Serialize(ResponseWrapper<object>.ErrorResponse([$"Invalid data for '{SUBSCRIPTION_HEADER}' received. Expect GUID"]), _responseSerializerOption);
                await context.Response.WriteAsync(json);
            }
            else
            {
                var subscriptionEntity = await subscriptionService.GetByReferenceId(subscriptionReference);
                if (subscriptionEntity == null) JsonSerializerHelper.Serialize(ResponseWrapper<object>.ErrorResponse([$"Subscription '{subscriptionReference}' not found"]), _responseSerializerOption);

                if (!userContextAccessor.UserContext.IsAuthenticated)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = MediaContentType.Json;
                    var json = JsonSerializerHelper.Serialize(ResponseWrapper<object>.ErrorResponse([$"'{SUBSCRIPTION_HEADER}' can be used only when user is authenticated"]), _responseSerializerOption);
                    await context.Response.WriteAsync(json);
                }
                else if(subscriptionEntity!.OwnerUserId != userContextAccessor.UserContext.UserId)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = MediaContentType.Json;
                    var json = JsonSerializerHelper.Serialize(ResponseWrapper<object>.ErrorResponse([$"Subscription '{subscriptionReference}' not found"]), _responseSerializerOption);
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
