using System;

namespace Mondo
{
    public sealed class MondoApiException : Exception
    {
        public MondoApiException(string message) : base(message)
        {
        }
    }
}