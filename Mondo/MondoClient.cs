using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        /// <param name="baseUri">URL of the Mondo API to use, defaults to production.</param>
        /// <param name="accessToken">Your access token.</param>
        public MondoClient(string accessToken, string baseUri = "https://api.getmondo.co.uk")
            : this(new HttpClient {BaseAddress = new Uri(baseUri)}, accessToken)
        {
        }

        internal MondoClient(HttpClient httpClient, string accessToken)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        /// <summary>
        /// Your OAuth 2.0 access token.
        /// </summary>
        public string AccessToken => _httpClient.DefaultRequestHeaders.Authorization?.Parameter;

        /// <summary>
        /// Returns a list of accounts owned by the currently authorised user.
        /// </summary>
        public async Task<IList<Account>> GetAccountsAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"accounts");
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw MondoException.CreateFromApiResponse(response, body);
            }

            return JsonConvert.DeserializeObject<ListAccountsResponse>(body).Accounts;
        }

        /// <summary>
        /// Returns balance information for a specific account.
        /// </summary>
        /// <param name="accountId">The id of the account.</param>
        public async Task<Balance> GetBalanceAsync(string accountId)
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));

            HttpResponseMessage response = await _httpClient.GetAsync($"balance?account_id={accountId}");
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw MondoException.CreateFromApiResponse(response, body);
            }

            return JsonConvert.DeserializeObject<Balance>(body);
        }

        /// <summary>
        /// Returns an individual transaction, fetched by its id.
        /// </summary>
        /// <param name="transactionId">The transaction ID.</param>
        /// <param name="expand">Can be merchant.</param>
        public async Task<Transaction> GetTransactionAsync(string transactionId, string expand = null)
        {
            if (transactionId == null) throw new ArgumentNullException(nameof(transactionId));

            var sb = new StringBuilder($"transactions/{transactionId}");
            if (!string.IsNullOrWhiteSpace(expand))
            {
                sb.Append($"?expand[]={expand}");
            }

            HttpResponseMessage response = await _httpClient.GetAsync(sb.ToString());
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw MondoException.CreateFromApiResponse(response, body);
            }

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

            var sb = new StringBuilder($"transactions?account_id={accountId}{paginationOptions}");
            if (expand != null)
            {
                sb.Append($"&expand[]={expand}");
            }

            HttpResponseMessage response = await _httpClient.GetAsync(sb.ToString());
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw MondoException.CreateFromApiResponse(response, body);
            }

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
                throw MondoException.CreateFromApiResponse(response, body);
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
                {"type", "basic"}
            };

            if (!string.IsNullOrWhiteSpace(url))
            {
                formValues.Add("url", url);
            }

            foreach (var pair in @params)
            {
                formValues.Add($"params[{pair.Key}]", pair.Value);
            }

            HttpResponseMessage response = await _httpClient.PostAsync("feed", new FormUrlEncodedContent(formValues), cancellationToken);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw MondoException.CreateFromApiResponse(response, body);
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
                throw MondoException.CreateFromApiResponse(response, body);
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

            HttpResponseMessage response = await _httpClient.GetAsync($"webhooks?account_id={accountId}");
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw MondoException.CreateFromApiResponse(response, body);
            }

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
                throw MondoException.CreateFromApiResponse(response, body);
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
                throw MondoException.CreateFromApiResponse(response, body);
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
                throw MondoException.CreateFromApiResponse(response, body);
            }
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
