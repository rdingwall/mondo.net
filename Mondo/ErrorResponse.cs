using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// Mondo API error response message.
    /// </summary>
    public sealed class ErrorResponse
    {
        /// <summary>
        /// Error response code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Error response error.
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Error response error description.
        /// </summary>
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Error response message.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Error response parameters.
        /// </summary>
        [JsonProperty("params")]
        public IDictionary<string, string> Params { get; set; }
    }
}
