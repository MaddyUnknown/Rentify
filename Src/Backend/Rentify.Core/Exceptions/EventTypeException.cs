using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Exceptions
{
    public class EventTypeException : EventException
    {
        public EventTypeException() : base() { }
        public EventTypeException(string message) : base(message) { }
        public EventTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
