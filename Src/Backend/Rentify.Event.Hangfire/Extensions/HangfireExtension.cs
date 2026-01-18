using Hangfire;
using Hangfire.SqlServer;
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
        public static void AddHangfireProducerServices(this IServiceCollection services, Action<EventProducerSetupOptions> optionsSetupAction)
        {
            EventProducerSetupOptions options = new EventProducerSetupOptions();
            optionsSetupAction(options);

            if (options.ConnectionString == null) throw new ArgumentNullException(nameof(options.ConnectionString), HangfireConstants.ConnectionStringNotConfigurated);

            // Add hangfire dependency
            services.AddHangfire(c =>
            {
                c.UseSqlServerStorage(options.ConnectionString);
            });

            // Add producer service
            services.AddTransient<IMessagePublisher, HangfireMessagePublisher>();
        }

        public static void AddHangfireConsumerServices(this IServiceCollection services, Action<EventProcessorSetupOptions> optionsSetupAction)
        {
            EventProcessorSetupOptions options = new EventProcessorSetupOptions();
            optionsSetupAction(options);

            if (options.ConnectionString == null) throw new ArgumentNullException(nameof(options.ConnectionString), HangfireConstants.ConnectionStringNotConfigurated);
            if (options.WorkerCount == null) throw new ArgumentNullException(nameof(options.WorkerCount), HangfireConstants.WorkerCountNotConfigurated);
            if (options.WorkerPollingInterval == null) throw new ArgumentNullException(nameof(options.WorkerPollingInterval), HangfireConstants.WorkerCountNotConfigurated);


            // Add hangfire dependency
            services.AddHangfire(c =>
            {
                c.UseSqlServerStorage(options.ConnectionString, new SqlServerStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(options.WorkerPollingInterval.Value),
                    UseRecommendedIsolationLevel = true
                });
            });

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = options.WorkerCount;
            });
        }
    }
}
