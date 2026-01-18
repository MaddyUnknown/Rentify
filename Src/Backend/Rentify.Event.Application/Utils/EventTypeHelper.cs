using Rentify.Core.Abstractions.Events;
using Rentify.Core.Entities;
using Rentify.Core.Events;
using Rentify.Core.Exceptions;
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
        public static Type ResolveEventObjectType(string eventObjectType)
        {
            Type eventType = Type.GetType(eventObjectType) 
                ?? throw new EventTypeException(string.Format(EventOutboxConstants.EventObjectTypeNotFound, eventObjectType));
            
            if (!typeof(EventBase).IsAssignableFrom(eventType)) 
                throw new EventTypeException(string.Format(EventOutboxConstants.EventObjectNotOfBaseType, eventObjectType, nameof(EventBase)));

            return eventType;
        }
    }
}
