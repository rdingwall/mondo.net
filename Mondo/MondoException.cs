using System;

namespace Mondo
{
    /// <summary>
    /// Represents an error response from the Mondo API.
    /// </summary>
    public sealed class MondoException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MondoClient"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public MondoException(string message) : base(message)
        {
        }
    }
}