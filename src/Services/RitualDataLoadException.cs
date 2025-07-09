using System;

namespace RitualOS.Services
{
    /// <summary>
    /// Represents errors that occur during ritual data load operations.
    /// </summary>
    public class RitualDataLoadException : Exception
    {
        public RitualDataLoadException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
