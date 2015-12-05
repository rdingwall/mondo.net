using System.Diagnostics;
using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// Web hooks allow your application to receive real-time, push notification of events in an account.
    /// </summary>
    [DebuggerDisplay("[{Id,nq} {Url}]")]
    public sealed class Webhook
    {
        /// <summary>
        /// The account to list registered web hooks for.
        /// </summary>
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// The webhook's Id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The URL we will send notifications to.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}