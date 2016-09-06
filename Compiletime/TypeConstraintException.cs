using System;
using System.Runtime.Serialization;

namespace Soalize
{
    /// <summary>
    /// Represents violations of type constraints on user supplied generic type parameters.
    /// </summary>
    [Serializable]
    public class TypeConstraintException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see name="TypeConstraintException"/>.
        /// </summary>
        public TypeConstraintException()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see name="TypeConstraintException"/> with a specified error message.
        /// </summary>
        public TypeConstraintException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see name="TypeConstraintException"/> with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public TypeConstraintException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TypeConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}