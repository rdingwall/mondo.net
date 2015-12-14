using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Mondo.Messages;
using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// An authenticated Mondo API client.
    /// </summary>
    public sealed class MondoClient : IMondoClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MondoClient"/> class.
        /// </summary>
        /// <param name="httpClient">HttpClient to use.</param>
        /// <param name="clientId">Your client ID.</param>
        /// <param name="clientSecret">Tour client secret.</param>
        public MondoClient(HttpClient httpClient, string clientId, string clientSecret)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            if (clientSecret == null) throw new ArgumentNullException(nameof(clientSecret));

            _httpClient = httpClient;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MondoClient"/> class.
        /// </summary>
        /// <param name="url">URL of the Mondo API to use.</param>
        /// <param name="clientId">Your client ID.</param>
        /// <param name="clientSecret">Tour client secret.</param>
        public MondoClient(string url, string clientId, string clientSecret)
            : this(new HttpClient {BaseAddress = new Uri(url)}, clientId, clientSecret)
        {
        }

        /// <summary>
        /// Your OAuth 2.0 access token.
        /// </summary>
        public string AccessToken
        {
            get
            {
                return _httpClient.DefaultRequestHeaders.Authorization?.Parameter;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
            }
        }

        /// <summary>
        /// The time at which the current access token will expire (to limit the window of opportunity for attackers in the event an access token is compromised).
        /// </summary>
        public DateTimeOffset AccessTokenExpiresAt { get; private set; }

        /// <summary>
        /// Your client ID.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Your client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Refresh token necessary to “refresh” your access when your access token expires.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Your user ID.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Acquires an OAuth2.0 access token. An access token is tied to both your application (the client) and an individual Mondo user and is valid for several hours.
        /// </summary>
        /// <param name="username">The user’s email address.</param>
        /// <param name="password">The user’s password.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task RequestAccessTokenAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken))
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

            var now = DateTimeOffset.UtcNow;

            HttpResponseMessage response = await _httpClient.PostAsync("oauth2/token", new FormUrlEncodedContent(formValues), cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body);
            }

            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(body);

            AccessToken = accessTokenResponse.AccessToken;
            AccessTokenExpiresAt = now.AddSeconds(accessTokenResponse.ExpiresIn);
            RefreshToken = accessTokenResponse.RefreshToken;
            UserId = accessTokenResponse.UserId;
        }

        /// <summary>
        /// To limit the window of opportunity for attackers in the event an access token is compromised, access tokens expire after 6 hours. To gain long-lived access to a user’s account, it’s necessary to “refresh” your access when it expires using a refresh token. Only “confidential” clients are issued refresh tokens – “public” clients must ask the user to re-authenticate.
        /// 
        /// Refreshing an access token will invalidate the previous token, if it is still valid.Refreshing is a one-time operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task RefreshAccessTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var formValues = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"client_id", ClientId},
                {"client_secret", ClientSecret},
                {"refresh_token", RefreshToken}
            };

            var now = DateTimeOffset.Now;

            HttpResponseMessage response = await _httpClient.PostAsync("oauth2/token", new FormUrlEncodedContent(formValues), cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body);
            }

            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(body);

            AccessToken = accessTokenResponse.AccessToken;
            AccessTokenExpiresAt = now.AddSeconds(accessTokenResponse.ExpiresIn);
            RefreshToken = accessTokenResponse.RefreshToken;
        }

        /// <summary>
        /// Returns a list of accounts owned by the currently authorised user.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Account>> ListAccountsAsync()
        {
            string body = await _httpClient.GetStringAsync("accounts");
            return JsonConvert.DeserializeObject<ListAccountsResponse>(body).Accounts;
        }

        /// <summary>
        /// Returns balance information for a specific account.
        /// </summary>
        /// <param name="accountId">The id of the account.</param>
        public async Task<BalanceResponse> GetBalanceAsync(string accountId)
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));

            string body = await _httpClient.GetStringAsync($"balance?account_id={accountId}");
            return JsonConvert.DeserializeObject<BalanceResponse>(body);
        }

        /// <summary>
        /// Returns an individual transaction, fetched by its id.
        /// </summary>
        /// <param name="transactionId">The transaction ID.</param>
        /// <param name="expand">Can be merchant.</param>
        public async Task<Transaction> GetTransactionAsync(string transactionId, string expand = null)
        {
            if (transactionId == null) throw new ArgumentNullException(nameof(transactionId));

            string body = await _httpClient.GetStringAsync($"transactions/{transactionId}?expand[]={expand}");
            return JsonConvert.DeserializeObject<RetrieveTransactionResponse>(body).Transaction;
        }

        /// <summary>
        /// Returns a list of transactions on the user’s account.
        /// </summary>
        /// <param name="accountId">The account to retrieve transactions from.</param>
        /// <param name="expand">Can be merchant.</param>
        /// <param name="paginationOptions">This endpoint can be paginated.</param>
        public async Task<IList<Transaction>> GetTransactionsAsync(string accountId, string expand = null, PaginationOptions paginationOptions = null)
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));

            string body = await _httpClient.GetStringAsync($"transactions?account_id={accountId}&expand[]={expand}{paginationOptions}");
            return JsonConvert.DeserializeObject<ListTransactionsResponse>(body).Transactions;
        }

        /// <summary>
        /// You may store your own key-value annotations against a transaction in its metadata.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="metadata">Include each key you would like to modify. To delete a key, set its value to an empty string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <remarks>Metadata is private to your application.</remarks>
        public async Task<Transaction> AnnotateTransactionAsync(string transactionId, IDictionary<string, string> metadata, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (transactionId == null) throw new ArgumentNullException(nameof(transactionId));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));

            var formValues = new Dictionary<string, string>();
            foreach (var pair in metadata)
            {
                formValues.Add($"metadata[{pair.Key}]", pair.Value);
            }

            var httpRequestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), $"transactions/{transactionId}")
            {
                Content = new FormUrlEncodedContent(formValues)
            };

            HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body);
            }

            return JsonConvert.DeserializeObject<AnnotateTransactionResponse>(body).Transaction;
        }

        /// <summary>
        /// Creates a new feed item on the user’s feed.
        /// </summary>
        /// <param name="accountId">The account to create feed item for.</param>
        /// <param name="type">Type of feed item. Currently only basic is supported.</param>
        /// <param name="params">A map of parameters which vary based on type</param>
        /// <param name="url">A URL to open when the feed item is tapped. If no URL is provided, the app will display a fallback view based on the title &amp; body.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task CreateFeedItemAsync(string accountId, string type, IDictionary<string, string> @params, string url = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (@params == null) throw new ArgumentNullException(nameof(@params));

            var formValues = new Dictionary<string, string>
            {
                {"account_id", accountId},
                {"type", "basic"},
                {"url", url}
            };

            foreach (var pair in @params)
            {
                formValues.Add($"params[{pair.Key}]", pair.Value);
            }

            HttpResponseMessage response = await _httpClient.PostAsync("feed", new FormUrlEncodedContent(formValues), cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body);
            }
        }

        /// <summary>
        /// Each time a matching event occurs, we will make a POST call to the URL you provide. If the call fails, we will retry up to a maximum of 5 attempts, with exponential backoff.
        /// </summary>
        /// <param name="accountId">The account to receive notifications for.</param>
        /// <param name="url">The URL we will send notifications to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<Webhook> CreateWebhookAsync(string accountId, string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));
            if (url == null) throw new ArgumentNullException(nameof(url));

            var formValues = new Dictionary<string, string>
            {
                {"account_id", accountId},
                {"url", url}
            };

            HttpResponseMessage response = await _httpClient.PostAsync("webhooks", new FormUrlEncodedContent(formValues), cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body);
            }

            return JsonConvert.DeserializeObject<RegisterWebhookResponse>(body).Webhook;
        }

        /// <summary>
        /// List the web hooks registered on an account.
        /// </summary>
        /// <param name="accountId">The account to list registered web hooks for.</param>
        public async Task<IList<Webhook>> GetWebhooksAsync(string accountId)
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));

            string body = await _httpClient.GetStringAsync($"webhooks?account_id={accountId}");
            return JsonConvert.DeserializeObject<ListWebhooksResponse>(body).Webhooks;
        }

        /// <summary>
        /// When you delete a web hook, we will no longer send notifications to it.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task DeleteWebhookAsync(string webhookId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (webhookId == null) throw new ArgumentNullException(nameof(webhookId));

            await _httpClient.DeleteAsync($"webhooks/{webhookId}", cancellationToken);
        }


        /// <summary>
        /// Once you have obtained a URL for an attachment, either by uploading to the upload_url obtained from the upload endpoint above or by hosting a remote image, this URL can then be registered against a transaction. Once an attachment is registered against a transaction this will be displayed on the detail page of a transaction within the Mondo app.
        /// </summary>
        /// <param name="externalId">The id of the transaction to associate the attachment with.</param>
        /// <param name="fileUrl">The URL of the uploaded attachment.</param>
        /// <param name="fileType">The content type of the attachment.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<Attachment> CreateAttachmentAsync(string externalId, string fileUrl, string fileType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (externalId == null) throw new ArgumentNullException(nameof(externalId));
            if (fileUrl == null) throw new ArgumentNullException(nameof(fileUrl));
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));
            
            var formValues = new Dictionary<string, string>
            {
                {"external_id", externalId},
                {"file_type", fileType},
                {"file_url", fileUrl}
            };

            HttpResponseMessage response = await _httpClient.PostAsync("attachment/register", new FormUrlEncodedContent(formValues), cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body);
            }

            return JsonConvert.DeserializeObject<RegisterAttachmentResponse>(body).Attachment;
        }

        /// <summary>
        /// Uploads an attachment from a file stream.
        /// </summary>
        /// <param name="filename">The name of the file to be uploaded</param>
        /// <param name="fileType">The content type of the file</param>
        /// <param name="externalId">The id of the transaction to associate the attachment with.</param>
        /// <param name="stream">The file stream.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<Attachment> UploadAttachmentAsync(string filename, string fileType, string externalId, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));
            if (externalId == null) throw new ArgumentNullException(nameof(externalId));
            if (stream == null) throw new ArgumentNullException(nameof(stream));


            // 1. The first step when uploading an attachment is to obtain a temporary URL to which the file can be uploaded. 
            var formValues = new Dictionary<string, string>
            {
                {"file_name", filename},
                {"file_type", fileType}
            };

            HttpResponseMessage response = await _httpClient.PostAsync("attachment/upload", new FormUrlEncodedContent(formValues), cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body);
            }

            // 2. Once you have obtained a URL for an attachment upload to the upload_url
            var uploadAttachmentResponse = JsonConvert.DeserializeObject<UploadAttachmentResponse>(body);

            using (var uploadClient = new HttpClient())
            {
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileType);

                var streamUploadResponse = await uploadClient.PutAsync(uploadAttachmentResponse.UploadUrl, streamContent, cancellationToken);

                if (!streamUploadResponse.IsSuccessStatusCode)
                {
                    var errorMessage = await streamUploadResponse.Content.ReadAsStringAsync();
                    throw new MondoException(streamUploadResponse.StatusCode, $"Error uploading file: {errorMessage}");
                }
            }

            // 3. Finally this URL can then be registered against a transaction
            return await CreateAttachmentAsync(externalId, uploadAttachmentResponse.FileUrl, fileType, cancellationToken);
        }

        /// <summary>
        /// To remove an attachment, simply deregister this using its id
        /// </summary>
        /// <param name="id">The id of the attachment to deregister.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task DeleteAttachmentAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var formValues = new Dictionary<string, string>
            {
                {"id", id}
            };

            HttpResponseMessage response = await _httpClient.PostAsync("attachment/deregister", new FormUrlEncodedContent(formValues), cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CreateException(response, body);
            }
        }

        /// <summary>
        /// Disposes the underlying HttpClient.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private static MondoException CreateException(HttpResponseMessage response, string body)
        {
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(body);
                return new MondoException(response.StatusCode, errorResponse.Message, errorResponse);
            }
            catch
            {
                return new MondoException(response.StatusCode, body);
            }
        }
    }
}
