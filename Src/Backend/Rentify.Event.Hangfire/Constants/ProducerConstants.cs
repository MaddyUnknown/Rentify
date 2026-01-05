using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Hangfire.Constants
{
    public static class ProducerConstants
    {
        public static readonly string NoEventHandlerFound = "No event handler found for event type: '{0}'";
        public static readonly string ConnectionStringNotConfigurated = "Connection string not configured for data access";
    }
}
