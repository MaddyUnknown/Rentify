using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Exceptions
{
    public class EventDataException : EventException
    {
        public EventDataException() : base() { }
        public EventDataException(string message) : base(message) { }
        public EventDataException(string message, Exception innerException) : base(message, innerException) { }
    }
}
