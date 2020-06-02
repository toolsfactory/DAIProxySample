using System;
using System.Runtime.Serialization;

namespace DAIProxy.Core
{
    [Serializable]
    internal class EncryptionException : Exception
    {
        public EncryptionException()
        {
        }

        public EncryptionException(string message) : base(message)
        {
        }

        public EncryptionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EncryptionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}