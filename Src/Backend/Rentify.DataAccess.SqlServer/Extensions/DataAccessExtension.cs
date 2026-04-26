using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.DataAccess.SqlServer.Data;
using Rentify.DataAccess.SqlServer.Options;
using Rentify.DataAccess.SqlServer.Repositories;
using UOW = Rentify.DataAccess.SqlServer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentify.DataAccess.SqlServer.Interceptors;
using Rentify.DataAccess.SqlServer.Interfaces.Interceptors;
using Rentify.DataAccess.SqlServer.Constants;
using Rentify.DataAccess.SqlServer.Interfaces.Filters;
using Rentify.DataAccess.SqlServer.Filters;

namespace Rentify.DataAccess.SqlServer.Extensions
{
    public static class DataAccessExtension
    {
        public static void AddDataAccessServices(this IServiceCollection serviceCollection, Action<DataAccessSetupOptions> optionsSetupAction)
        {
            DataAccessSetupOptions dataAccessOptions = new DataAccessSetupOptions();
            optionsSetupAction(dataAccessOptions);

            if(dataAccessOptions.ConnectionString == null) throw new ArgumentNullException(nameof(dataAccessOptions.ConnectionString), DataAccessConstant.ConnectionStringNotConfigurated);

            // Add Rentify DbContext
            serviceCollection.AddDbContext<RentifyDbContext>(options =>
            {
                options.UseSqlServer(dataAccessOptions.ConnectionString);
            });

            // Add Save Changes Interceptor
            serviceCollection.AddSingleton<ISaveChangesInterceptor, SetAuditFieldsInterceptor>();
            serviceCollection.AddSingleton<ISaveChangesInterceptor, SetOwnerFieldsInterceptor>();

            // Add Global Filter
            serviceCollection.AddSingleton<IGlobalFilter, GlobalSubscriptionFilter>();

            // Add Repository
            serviceCollection.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            serviceCollection.AddTransient<IMediaFileRepository, MediaFileRepository>();
            serviceCollection.AddTransient<IMediaFileVariantRepository, MediaFileVariantRepository>();
            serviceCollection.AddTransient<IUnitRepository, UnitRepository>();
            serviceCollection.AddTransient<IPropertyRepository, PropertyRepository>();
            serviceCollection.AddTransient<ISubscriptionRepository>(provider => new SubscriptionRepository(dataAccessOptions.ConnectionString));
            serviceCollection.AddTransient<ITenantEmergencyContactRepository, TenantEmergencyContactRepository>();
            serviceCollection.AddTransient<ITenantRepository, TenantRepository>();


            // Add unit of work
            serviceCollection.AddTransient<IUnitOfWork, UOW.UnitOfWork>();
        }
    }
}
