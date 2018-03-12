using System;

namespace Monzo.Extensions
{
    internal static class DateTimeExtensions
    {
        public static string ToRfc3339String(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
        }
    }
}
