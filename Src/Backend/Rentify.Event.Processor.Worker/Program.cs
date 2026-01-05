using Rentify.DataAccess.SqlServer.Extensions;
using Rentify.Event.Application.Extensions;
using Rentify.Event.Hangfire.Extensions;
using Rentify.Storage.LocalStorage.Extensions;

namespace Rentify.Event.Processor.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Add message handler services
            builder.Services.AddMessageHandlerServices();

            // Add data access services
            builder.Services.AddDataAccessServices(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationDBConnection");
            });

            // Add event processing services
            builder.Services.AddHangfireConsumerServices(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("HangfireDBConnection");
                options.WorkerPollingInterval = builder.Configuration.GetValue<double>("WorkerPollingInterval");
                options.WorkerCount = Environment.ProcessorCount * 2;
            });

            // Add storage services
            builder.Services.AddLocalStorageServices(options =>
            {
                options.RootFolder = builder.Configuration.GetValue<string>("Storage:RootFolder");
                options.StreamBufferSize = builder.Configuration.GetValue<int>("Storage:StreamBufferSize");
            });

            var host = builder.Build();
            host.Run();
        }
    }
}