using System;
using System.Runtime.Serialization;

namespace Rollover.Ib
{
    [Serializable]
    internal class LastUnderlyingPriceException : Exception
    {
        public LastUnderlyingPriceException()
        {
        }

        public LastUnderlyingPriceException(string message) : base(message)
        {
        }

        public LastUnderlyingPriceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LastUnderlyingPriceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}