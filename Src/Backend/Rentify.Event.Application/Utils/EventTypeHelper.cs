using Rentify.Core.Abstractions.Events;
using Rentify.Core.Entities;
using Rentify.Core.Events;
using Rentify.Event.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Event.Utils
{
    public static class EventTypeHelper
    {
        public static Type ResolveEventObjectType(int eventId, string eventObjectType)
        {
            Type eventType = Type.GetType(eventObjectType) 
                ?? throw new InvalidOperationException(string.Format(EventOutboxConstants.EventObjectTypeNotFound, eventObjectType, eventId));
            
            if (!typeof(EventBase).IsAssignableFrom(eventType)) 
                throw new InvalidOperationException(string.Format(EventOutboxConstants.EventObjectNotOfBaseType, eventObjectType, nameof(EventBase), eventId));

            return eventType;
        }
    }
}
