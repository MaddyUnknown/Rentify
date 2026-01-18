using Hangfire;
using Hangfire.Server;
using Rentify.Core.Abstractions.Events;
using Rentify.Event.Core;
using Rentify.Event.Hangfire.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Hangfire
{
    public class HangfireDispatcher<TMessage> where TMessage : EventBase
    {
        private readonly IMessageHandler<TMessage> _handler;

        public HangfireDispatcher(IMessageHandler<TMessage> handler)
        {
            _handler = handler;
        }

        public async Task DispatchAsync(TMessage message, PerformContext context)
        {
            var currentRetryCount = context.GetJobParameter<int>("RetryCount");
            var maxRetry = AutomaticRetryAttribute.DefaultRetryAttempts;

            var messageContext = new HangfireMessageProcessingContext(currentRetryCount, maxRetry);
            await _handler.HandleAsync(message, messageContext);
        }
    }
}
