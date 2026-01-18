using Hangfire;
using Hangfire.Server;
using Rentify.Core.Abstractions.Events;
using Rentify.Event.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Hangfire
{
    public class HangfireMessagePublisher: IMessagePublisher
    {
        private IBackgroundJobClient _jobClient;

        public HangfireMessagePublisher(IBackgroundJobClient backgroundJobClient)
        {
            _jobClient = backgroundJobClient;
        }

        public Task PublishAsync<TMessage>(TMessage message) where TMessage : EventBase
        {
            _jobClient.Enqueue<HangfireDispatcher<TMessage>>(d => d.DispatchAsync(message, null!));
            return Task.CompletedTask;
        }
    }
}
