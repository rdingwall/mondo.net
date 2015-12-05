using System;

namespace Mondo
{
    public sealed class MondoException : Exception
    {
        public MondoException(string message) : base(message)
        {
        }
    }
}