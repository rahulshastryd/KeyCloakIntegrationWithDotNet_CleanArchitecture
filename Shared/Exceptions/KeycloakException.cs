using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    [Serializable]
    public class KeycloakException : Exception
    {
        public KeycloakException()
        {
        }

        public KeycloakException(string? message) : base(message)
        {
        }

        public KeycloakException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected KeycloakException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
