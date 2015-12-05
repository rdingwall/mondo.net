using System;
using Mondo.Extensions;

namespace Mondo
{
    /// <summary>
    /// Endpoints which enumerate objects support time-based and cursor-based pagination.
    /// </summary>
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

        /// <summary>
        /// Format the pagination options as a query string.
        /// </summary>
        public override string ToString()
        {
            string since = SinceTime?.ToRfc3339String() ?? SinceId;
            string before = BeforeTime?.ToRfc3339String();

            return $"&limit={Limit}&since={since}&before={before}";
        }
    }
}
