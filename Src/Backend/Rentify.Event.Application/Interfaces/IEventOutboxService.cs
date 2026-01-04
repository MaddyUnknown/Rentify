using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Interfaces
{
    public interface IEventOutboxService
    {
        Task PublishEventsAsync(EventOutbox eventOutbox);
    }
}
