using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Core.Exceptions
{
    public class AppValidationException : AppException
    {
        private readonly List<string> _errors;


        public AppValidationException(string message) : base(message)
        {
            _errors = new List<string>();
            _errors.Add(message);
        }

        public AppValidationException(string message, Exception innerException) : base(message, innerException)
        {
            _errors = new List<string>();
            _errors.Add(message);
        }

        public AppValidationException(IEnumerable<string> messages) : base(string.Join(", ", messages)) {
            _errors = new List<string>(messages);
        }

        public AppValidationException(IEnumerable<string> messages, Exception innerException) : base(string.Join(", ", messages), innerException)
        {
            _errors = new List<string>(messages);
        }


        public IReadOnlyList<string> Errors => _errors;
    }
}
