using Microsoft.Extensions.Logging;
using Rentify.Core.Abstractions.Events;
using Rentify.Core.Entities;
using Rentify.Core.Events;
using Rentify.Core.Exceptions;
using Rentify.Core.Utils;
using Rentify.Event.Application.Constants;
using Rentify.Event.Application.Event.Utils;
using Rentify.Event.Application.Interfaces;
using Rentify.Event.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Services
{
    public class EventOutboxService : IEventOutboxService
    {
        private readonly ConcurrentDictionary<Type, Func<EventOutbox, Task>> _handlerCache;
        private readonly IMessagePublisher _publisher;

        public EventOutboxService(IMessagePublisher publisher)
        {
            _handlerCache = new ConcurrentDictionary<Type, Func<EventOutbox, Task>>();
            _publisher = publisher;
        }

        public async Task PublishEventsAsync(EventOutbox eventOutbox)
        {
            var eventObjectType = EventTypeHelper.ResolveEventObjectType(eventOutbox.EventObjectType);
            var handler = _handlerCache.GetOrAdd(eventObjectType, CreatePublishEventHandler(eventObjectType));
            await handler(eventOutbox);
        }


        private async Task PublishEventHandlerAsync<T>(EventOutbox eventOutbox) where T : EventBase
        {
            var data = JsonSerializerHelper.Deserialize<T>(eventOutbox.EventData);
            if (data == null) throw new EventDataException(string.Format(EventOutboxConstants.DeserializeEventOutboxPayloadError, eventOutbox.EventData, typeof(T)));

            await _publisher.PublishAsync(data);
        }

        private Func<EventOutbox, Task> CreatePublishEventHandler(Type eventType)
        {
            var method = typeof(EventOutboxService).GetMethod(nameof(PublishEventHandlerAsync), BindingFlags.NonPublic | BindingFlags.Instance )!.MakeGenericMethod(eventType);
            return (eventOutbox) => (Task) method.Invoke(this, new object[] { eventOutbox })!;
        }
    }
}
