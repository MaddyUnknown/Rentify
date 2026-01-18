using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Exceptions
{
    public class EventException : AppException
    {
        public EventException() : base() { }
        public EventException(string message) : base(message) { }
        public EventException(string message, Exception innerException) : base(message, innerException) { }
    }
}
