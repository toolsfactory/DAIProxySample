using System;
using System.Runtime.Serialization;

namespace DAIProxy.Core
{
    [Serializable]
    internal class DecodingException : Exception
    {
        public DecodingException()
        {
        }

        public DecodingException(string message) : base(message)
        {
        }

        public DecodingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DecodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}