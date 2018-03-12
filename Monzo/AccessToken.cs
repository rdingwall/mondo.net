using System.Diagnostics;
using Newtonsoft.Json;

namespace Monzo
{
    /// <summary>
    /// Monzo access token response.
    /// </summary>
    [DebuggerDisplay("[{Value,nq}]")]
    public sealed class AccessToken
    {
        /// <summary>
        /// The Access Token to use for requests.
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string Value { get; set; }

        /// <summary>
        /// Your client ID.
        /// </summary>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// The OAuth 2 token type.
        /// </summary>
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Number of seconds before the token expires.
        /// </summary>
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// The OAuth refresh token to use to grant a new access token.
        /// </summary>
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// The user ID.
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }
    }
}
