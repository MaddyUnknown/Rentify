using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Rentify.Event.Core;
using Rentify.Event.Hangfire.Constants;
using Rentify.Event.Hangfire.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rentify.Event.Hangfire.Extensions
{
    public static class HangfireExtension
    {
        public static void AddHangfireProducerServices(this IServiceCollection services, Action<EventProcessingSetupOptions> optionsSetupAction)
        {
            EventProcessingSetupOptions dataAccessOptions = new EventProcessingSetupOptions();
            optionsSetupAction(dataAccessOptions);

            if (dataAccessOptions.ConnectionString == null) throw new ArgumentNullException(nameof(dataAccessOptions.ConnectionString), ProducerConstants.ConnectionStringNotConfigurated);

            // Add hangfire dependency
            services.AddHangfire(c =>
            {
                c.UseSqlServerStorage(dataAccessOptions.ConnectionString);
            });

            // Add producer service
            services.AddTransient<IMessagePublisher, HangfireMessagePublisher>();
        }

        public static void AddHangfireConsumerServices(this IServiceCollection services, Action<EventProcessingSetupOptions> optionsSetupAction)
        {
            EventProcessingSetupOptions dataAccessOptions = new EventProcessingSetupOptions();
            optionsSetupAction(dataAccessOptions);

            if (dataAccessOptions.ConnectionString == null) throw new ArgumentNullException(nameof(dataAccessOptions.ConnectionString), ProducerConstants.ConnectionStringNotConfigurated);

            // Add hangfire dependency
            services.AddHangfire(c =>
            {
                c.UseSqlServerStorage(dataAccessOptions.ConnectionString);
            });

            services.AddHangfireServer();
        }
    }
}
