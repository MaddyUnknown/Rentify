using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Rentify.API.Controllers;
using Rentify.API.Middlewares;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rentify.API.Swagger.OperationFilters
{
    public class SubscriptionHeaderOperationFilter : IOperationFilter
    {
        private readonly IList<string> _targetBaseRoute;

        public SubscriptionHeaderOperationFilter()
        {
            _targetBaseRoute = new List<string>()
            {
                PropertiesController.BASE_ROUTE,
                TenantController.BASE_ROUTE,
                MediaController.BASE_ROUTE,
            };
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            if (!_targetBaseRoute.Any(r => context.ApiDescription.RelativePath?.StartsWith(r) ?? false)) return;

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = DataScopeMiddleware.SUBSCRIPTION_HEADER,
                In = ParameterLocation.Header,
                Required = false, // set true if mandatory
                Description = "Custom header to add subscription currently used by user",
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}
