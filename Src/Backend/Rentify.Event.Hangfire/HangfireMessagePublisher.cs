using Hangfire;
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
        public Task PublishAsync<TMessage>(TMessage message) where TMessage : EventBase
        {
            BackgroundJob.Enqueue<HangfireDispatcher<TMessage>>(d => d.DispatchAsync(message));
            return Task.CompletedTask;
        }
    }
}
