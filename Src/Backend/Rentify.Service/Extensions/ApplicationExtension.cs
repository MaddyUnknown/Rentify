using Microsoft.Extensions.DependencyInjection;
using Rentify.Application.Interfaces.Resolvers;
using Rentify.Application.Interfaces.Services;
using Rentify.Application.Interfaces.Validators;
using Rentify.Application.Resolvers;
using Rentify.Application.Services;
using Rentify.Application.Validators.MediaFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Extensions
{
    public static class ApplicationExtension
    {
        public static void AddApplicationServices(this IServiceCollection serviceCollection)
        {
            // Add Validator
            serviceCollection.AddTransient<IMediaFileValidator, PropertyMediaFileValidator>();

            // Add Resolver
            serviceCollection.AddTransient<IMediaFileValidatorResolver, MediaFileValidatorResolver>();

            // Add Service
            serviceCollection.AddScoped<IMediaFileService, MediaFileService>();
            serviceCollection.AddScoped<IPropertyService, PropertyService>();
            serviceCollection.AddScoped<IUnitService, UnitService>();
        }
    }
}
