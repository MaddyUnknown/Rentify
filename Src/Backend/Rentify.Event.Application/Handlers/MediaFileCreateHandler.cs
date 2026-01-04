using Rentify.Core.Events;
using Rentify.Event.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Event.Application.Handlers
{
    public class MediaFileCreateHandler : IMessageHandler<MediaFileCreateEvent>
    {
        public Task HandleAsync(MediaFileCreateEvent message)
        {
            return Task.CompletedTask;
        }
    }
}
