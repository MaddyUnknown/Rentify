using Rentify.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Core.Exceptions
{
    public class AppInvalidCredentialException : AppAuthException
    {
        public AppInvalidCredentialException() : base("Invalid credential") { }
        public AppInvalidCredentialException(string message) : base(message) { }
        public AppInvalidCredentialException(string message, Exception innerException) : base(message, innerException) { }
    }
}
