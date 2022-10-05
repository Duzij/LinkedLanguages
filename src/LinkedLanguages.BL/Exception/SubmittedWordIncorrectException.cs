using System;
using System.Runtime.Serialization;

namespace LinkedLanguages.BL.Exception
{
    [Serializable]
    public class SubmittedWordIncorrectException : System.Exception
    {
        public SubmittedWordIncorrectException()
        {
        }

        public SubmittedWordIncorrectException(string message) : base(message)
        {
        }

        public SubmittedWordIncorrectException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected SubmittedWordIncorrectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
