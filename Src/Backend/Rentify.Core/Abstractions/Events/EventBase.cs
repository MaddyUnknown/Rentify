using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Abstractions.Events
{
    public class EventBase
    {
        private Guid _id = new Guid();
        public Guid EventId => _id;
    }
}
