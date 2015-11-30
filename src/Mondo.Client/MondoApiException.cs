using System;

namespace Mondo.Client
{
    public sealed class MondoApiException : Exception
    {
        public MondoApiException(string message) : base(message)
        {
        }
    }
}