using Rentify.Core.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Abstractions.Events
{
    public abstract class EventBase
    {
        public Guid EventId { get; protected set; }

        protected EventBase()
        {
            if(EventId == Guid.Empty) EventId = Guid.NewGuid();
        }
    }
}
