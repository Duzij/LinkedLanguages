using System;
using System.Runtime.Serialization;

namespace LinkedLanguages.BL
{
    [Serializable]
    public class WordNotFoundException : Exception
    {
        public WordNotFoundException()
        {
        }

        public WordNotFoundException(string message) : base(message)
        {
        }

        public WordNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WordNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}