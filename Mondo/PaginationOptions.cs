using System;
using Mondo.Extensions;

namespace Mondo
{
    public sealed class PaginationOptions
    {
        /// <summary>
        /// Limits the number of results per-page.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// An RFC 3339-encoded timestamp.
        /// </summary>
        public DateTimeOffset? SinceTime { get; set; }

        /// <summary>
        /// Or an object id.
        /// </summary>
        public string SinceId { get; set; }

        /// <summary>
        /// An RFC 3339 encoded-timestamp
        /// </summary>
        public DateTimeOffset? BeforeTime { get; set; }

        public override string ToString()
        {
            string since = SinceTime?.ToRfc3339String() ?? SinceId;
            string before = BeforeTime?.ToRfc3339String();

            return $"&limit={Limit}&since={since}&before={before}";
        }
    }
}
