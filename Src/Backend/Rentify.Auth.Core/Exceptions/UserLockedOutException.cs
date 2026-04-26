using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Auth.Core.Exceptions
{
    public class UserLockedOutException : AppAuthException
    {
        public DateTime LockedTillDateTime { get; private set; }

        public UserLockedOutException(DateTime lockedTillDateTime) : base($"Used locked till '{lockedTillDateTime}'")
        {
            LockedTillDateTime = lockedTillDateTime;
        }
        public UserLockedOutException(DateTime lockedTillDateTime, string message) : base(message)
        {
            LockedTillDateTime = lockedTillDateTime;
        }
        public UserLockedOutException(DateTime lockedTillDateTime, string message, Exception innerException) : base(message, innerException)
        {
            LockedTillDateTime = lockedTillDateTime;
        }
    }
}
