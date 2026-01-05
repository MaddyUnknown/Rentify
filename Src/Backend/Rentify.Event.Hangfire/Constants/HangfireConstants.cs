using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Hangfire.Constants
{
    public static class HangfireConstants
    {
        public static readonly string NoEventHandlerFound = "No event handler found for event type: '{0}'";
        public static readonly string ConnectionStringNotConfigurated = "Connection string not configured for hangfire data access";
        public static readonly string WorkerCountNotConfigurated = "Worker count not configured for hangfire server";
        public static readonly string WorkerPollingIntervalNotConfigurated = "Worker polling interval not configured for hangfire server";
    }
}
