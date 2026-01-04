using Rentify.Core.Abstractions.Events;
using Rentify.Event.Core;
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

        public Task DispatchAsync(TMessage message) => _handler.HandleAsync(message);
    }
}
