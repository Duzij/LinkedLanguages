using System;
using System.Runtime.Serialization;

namespace LinkedLanguages.BL.Exception
{
    [Serializable]
    public class WordNotFoundException : System.Exception
    {
        public WordNotFoundException()
        {
        }

        public WordNotFoundException(string message) : base(message)
        {
        }

        public WordNotFoundException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected WordNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}