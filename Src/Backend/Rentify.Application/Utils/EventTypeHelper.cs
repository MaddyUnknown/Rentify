using Rentify.Application.Constants;
using Rentify.Core.Events;
using Rentify.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Utils
{
    public static class EventTypeHelper
    {
        public static string GetEventObjectType<T>()
        {
            return typeof(T).AssemblyQualifiedName ?? throw new EventTypeException(string.Format(EventOutboxConstants.NullEventObjectType, typeof(T)));
        }
    }
}
