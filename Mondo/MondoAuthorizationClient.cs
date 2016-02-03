using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// Mondo API authorization client.
    /// </summary>
    public sealed class MondoAuthorizationClient : IMondoAuthorizationClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Your client ID.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Your client secret.
        /// </summary>
        public string ClientSecret { get; }

        /// <summary>
        /// Initialises a new <see cref="MondoAuthorizationClient"/>
        /// </summary>
        /// <param name="clientId">Your client ID.</param>
        /// <param name="clientSecret">Tour client secret.</param>
        /// <param name="baseUri">URL of the Mondo API to use, defaults to production.</param>
        public MondoAuthorizationClient(string clientId, string clientSecret, string baseUri = "https://api.getmondo.co.uk") 
            : this(new HttpClient { BaseAddress = new Uri(baseUri) }, clientId, clientSecret)
        {
        }

        /// <summary>
        /// Initialises a new <see cref="MondoAuthorizationClient"/>
        /// </summary>
        /// <param name="httpClient">HttpClient to use.</param>
        /// <param name="clientId">Your client ID.</param>
        /// <param name="clientSecret">Tour client secret.</param>
        internal MondoAuthorizationClient(HttpClient httpClient, string clientId, string clientSecret)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Parameter is required", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("Parameter is required", nameof(clientSecret));

            _httpClient = httpClient;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// Generates a Mondo OAuth authorization URL based on state and redirect.
        /// </summary>
        /// <param name="state">State which will be passed back to you to prevent tampering.</param>
        /// <param name="redirectUri">The URI we will redirect back to after an authorization by the resource owner.</param>
        /// <returns> Returns the OAuth authorization URL. </returns>
        public string GetAuthorizeUrl(string state = null, string redirectUri = null)
        {
            var sb = new StringBuilder("https://auth.getmondo.co.uk/?response_type=code&client_id=");
            sb.Append(ClientId);

            if (!string.IsNullOrWhiteSpace(state))
            {
                sb.Append("&state=");
                sb.Append(state);
            }

            if (!string.IsNullOrWhiteSpace(redirectUri))
            {
                sb.Append("&redirect_uri=");
                sb.Append(WebUtility.UrlEncode(redirectUri));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Exchange this authorization code for an AccessToken, allowing requests to be mande on behalf of a user.
        /// </summary>
        /// <param name="authorizationCode">The authorization code.</param>
        /// <param name="redirectUri">The URL the user should be redrected back to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns the <see cref="AccessToken"/>.</returns>
        public async Task<AccessToken> GetAccessTokenAsync(string authorizationCode, string redirectUri, CancellationToken cancellationToken = new CancellationToken())
        {
            var formValues = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "grant_type", "authorization_code" },
                { "code", authorizationCode },
                { "redirect_uri", redirectUri }
            };

            return await AuthorizeAsync(formValues, cancellationToken);
        }

        /// <summary>
        /// Acquires an OAuth2.0 access token. An access token is tied to both your application (the client) and an individual Mondo user and is valid for several hours.
        /// </summary>
        /// <param name="username">The user’s email address.</param>
        /// <param name="password">The user’s password.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<AccessToken> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = new CancellationToken())
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            if (password == null) throw new ArgumentNullException(nameof(password));

            var formValues = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", ClientId},
                {"client_secret", ClientSecret},
                {"username", username},
                {"password", password}
            };

            return await AuthorizeAsync(formValues, cancellationToken);
        }

        /// <summary>
        /// Exchange this authorization code for an AccessToken, allowing requests to be mande on behalf of a user.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns an <see cref="AccessToken"/>.</returns>
        public async Task<AccessToken> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken())
        {
            var formValues = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            return await AuthorizeAsync(formValues, cancellationToken);
        }

        private async Task<AccessToken> AuthorizeAsync(Dictionary<string, string> formValues, CancellationToken cancellationToken = new CancellationToken())
        {
            HttpResponseMessage response = await _httpClient.PostAsync("oauth2/token", new FormUrlEncodedContent(formValues), cancellationToken).ConfigureAwait(false);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw MondoException.CreateFromApiResponse(response, body);
            }

            return JsonConvert.DeserializeObject<AccessToken>(body);
        }

        /// <summary>
        /// Disposes the underlying HttpClient.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
