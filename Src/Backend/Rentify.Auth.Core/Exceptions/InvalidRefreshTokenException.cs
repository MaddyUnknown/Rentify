using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Core.Exceptions
{
    public class InvalidRefreshTokenException : AppAuthException
    {
        public InvalidRefreshTokenException() : base("Invalid refresh token") { }
        public InvalidRefreshTokenException(string message) : base(message) { }
        public InvalidRefreshTokenException(string message, Exception innerException) : base(message, innerException) { }
    }
}
