using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Constants
{
    public static class EventOutboxConstants
    {
        public static readonly string DeserializeEventOutboxPayloadError = "Unable to deserialize event payload '{0}' to type '{1}'";
        public static readonly string EventOutboxNotSupported = "Handler not registered for outbox type '{0}'";
        public static readonly string EventObjectTypeNotFound = "Event object type '{0}' for event outbox not found";
        public static readonly string EventObjectNotOfBaseType = "Event object type '{0}' is not of base type '{1}'";
    }
}
