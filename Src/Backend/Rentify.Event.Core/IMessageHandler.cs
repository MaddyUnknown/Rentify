using Rentify.Core.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Core
{
    public interface IMessageHandler<in TMessage> where TMessage : EventBase
    {
        Task HandleAsync(TMessage message);
    }
}
