using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mondo
{
    /// <summary>
    /// Mondo API authorization client.
    /// </summary>
    public interface IMondoAuthorizationClient : IDisposable
    {
        /// <summary>
        /// Your client ID.
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Your client secret.
        /// </summary>
        string ClientSecret { get; }

        /// <summary>
        /// Generates a Mondo OAuth authorization URL based on state and redirect.
        /// </summary>
        /// <param name="state">State which will be passed back to you to prevent tampering.</param>
        /// <param name="redirectUri">The URI we will redirect back to after an authorization by the resource owner.</param>
        /// <returns> Returns the OAuth authorization URL. </returns>
        string GetAuthorizeUrl(string state = null, string redirectUri = null);

        /// <summary>
        /// Exchange this authorization code for an AccessToken, allowing requests to be mande on behalf of a user.
        /// </summary>
        /// <param name="authorizationCode">The authorization code.</param>
        /// <param name="redirectUri">The URL the user should be redrected back to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns the <see cref="AccessToken"/>.</returns>
        Task<AccessToken> GetAccessTokenAsync(string authorizationCode, string redirectUri, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Acquires an OAuth2.0 access token. An access token is tied to both your application (the client) and an individual Mondo user and is valid for several hours.
        /// </summary>
        /// <param name="username">The user’s email address.</param>
        /// <param name="password">The user’s password.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task<AccessToken> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Exchange this authorization code for an AccessToken, allowing requests to be mande on behalf of a user.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Returns an <see cref="AccessToken"/>.</returns>
        Task<AccessToken> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = new CancellationToken());
    }
}