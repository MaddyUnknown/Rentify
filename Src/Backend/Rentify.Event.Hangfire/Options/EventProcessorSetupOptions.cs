using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Hangfire.Options
{
    public class EventProcessorSetupOptions
    {
        public string? ConnectionString { get; set; }
        public int? WorkerCount { get; set; }
        public double? WorkerPollingInterval { get; set; }
    }
}
