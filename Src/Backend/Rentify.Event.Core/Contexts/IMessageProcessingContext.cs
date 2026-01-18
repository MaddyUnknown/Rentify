using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Core.Contexts
{
    public interface IMessageProcessingContext
    {
        int CurrentRetry { get; }
        int MaxRetry { get; }
        bool IsLastRetry { get; }
    }
}
