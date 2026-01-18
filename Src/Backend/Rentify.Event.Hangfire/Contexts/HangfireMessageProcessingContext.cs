using Rentify.Event.Core.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Hangfire.Contexts
{
    public class HangfireMessageProcessingContext : IMessageProcessingContext
    {
        public HangfireMessageProcessingContext(int currentRetry, int maxRetry)
        {
            CurrentRetry = currentRetry;
            MaxRetry = maxRetry;
        }

        public int CurrentRetry { get; private set; }

        public int MaxRetry { get; private set; }

        public bool IsLastRetry => CurrentRetry >= MaxRetry;
    }
}
