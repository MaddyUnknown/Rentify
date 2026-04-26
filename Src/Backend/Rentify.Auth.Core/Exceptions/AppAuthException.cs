using Rentify.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Core.Exceptions
{
    public class AppAuthException : AppException
    {
        public AppAuthException() : base() { }
        public AppAuthException(string message) : base(message) { }
        public AppAuthException(string message, Exception innerException) : base(message, innerException) { }
    }
}
