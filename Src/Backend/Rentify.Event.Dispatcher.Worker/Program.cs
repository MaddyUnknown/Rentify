using Rentify.DataAccess.SqlServer.Extensions;
using Rentify.Event.Application.Extensions;
using Rentify.Event.Hangfire.Extensions;

namespace Rentify.Event.Dispatcher.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<EventDispatcherWorker>();
            builder.Services.AddTransient<EventDispatcherProcessor>();

            // Add dispatcher services
            builder.Services.AddDispatcherServices();

            // Add data access services
            builder.Services.AddDataAccessServices(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationDBConnection");
            });

            // Add event processing services
            builder.Services.AddHangfireProducerServices(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("HangfireDBConnection");
            });

            var host = builder.Build();
            host.Run();
        }
    }
}