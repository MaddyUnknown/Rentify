using Rentify.Core.Entities;
using Rentify.DataAccess.Core.Repositories;
using Rentify.Event.Application.Interfaces;
using Rentify.Event.Application.Services;

namespace Rentify.Dispatcher.Worker
{
    public class EventDispatcherWorker : BackgroundService
    {
        private readonly int _interval;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventDispatcherWorker(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _interval = configuration.GetValue<int>("DispatcherProcessInterval");
            _serviceScopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<EventDispatcherProcessor>();
                    await processor.ExecuteAsync();
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
