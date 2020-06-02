using System;
using System.Runtime.Serialization;

namespace DAIProxy.Core
{
    [Serializable]
    internal class EncodingException : Exception
    {
        public EncodingException()
        {
        }

        public EncodingException(string message) : base(message)
        {
        }

        public EncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EncodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}