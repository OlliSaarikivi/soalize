using System;
using System.Runtime.Serialization;

namespace Soalize.Transpiler
{
    [Serializable]
    internal class TranspilationException : Exception
    {
        public TranspilationException()
        {
        }

        public TranspilationException(string message) : base(message)
        {
        }

        public TranspilationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TranspilationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}