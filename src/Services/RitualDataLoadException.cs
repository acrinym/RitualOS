using System;

namespace RitualOS.Services
{
    /// <summary>
    /// Represents errors that occur while reading or deserializing ritual data.
    /// </summary>
    public class RitualDataLoadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RitualDataLoadException"/> class.
        /// </summary>
        /// <param name="message">Description of the error.</param>
        /// <param name="innerException">Optional underlying exception that caused the failure.</param>
        public RitualDataLoadException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
        public RitualDataLoadException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
