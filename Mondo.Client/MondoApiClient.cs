using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mondo.Client.Messages;
using Newtonsoft.Json;

namespace Mondo.Client
{
    public sealed class MondoApiClient : IMondoApiClient
    {
        private readonly HttpClient _httpClient;

        public MondoApiClient(HttpClient httpClient, string clientId, string clientSecret)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            if (clientSecret == null) throw new ArgumentNullException(nameof(clientSecret));

            _httpClient = httpClient;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public MondoApiClient(string url, string clientId, string clientSecret)
            : this(new HttpClient {BaseAddress = new Uri(url)}, clientId, clientSecret)
        {
        }

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

        public DateTimeOffset AccessTokenTimestamp { get; private set; }

        public TimeSpan ExpiresIn { get; private set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RefreshToken { get; set; }

        public string UserId { get; private set; }

        public async Task RequestAccessTokenAsync(string username, string password)
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

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("oauth2/token", new FormUrlEncodedContent(formValues));
            string body = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }

            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(body);

            AccessToken = accessTokenResponse.AccessToken;
            AccessTokenTimestamp = DateTimeOffset.UtcNow;
            ExpiresIn = TimeSpan.FromSeconds(accessTokenResponse.ExpiresIn);
            RefreshToken = accessTokenResponse.RefreshToken;
            UserId = accessTokenResponse.UserId;
        }

        public async Task RefreshAccessTokenAsync()
        {
            var formValues = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"client_id", ClientId},
                {"client_secret", ClientSecret},
                {"refresh_token", RefreshToken}
            };

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("oauth2/token", new FormUrlEncodedContent(formValues));
            string body = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }

            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(body);

            AccessToken = accessTokenResponse.AccessToken;
            AccessTokenTimestamp = DateTimeOffset.UtcNow;
            ExpiresIn = TimeSpan.FromSeconds(accessTokenResponse.ExpiresIn);
            RefreshToken = accessTokenResponse.RefreshToken;
        }

        public async Task<IList<Account>> ListAccountsAsync()
        {
            string body = await _httpClient.GetStringAsync("accounts");
            return JsonConvert.DeserializeObject<ListAccountsResponse>(body).Accounts;
        }

        public async Task<Transaction> RetrieveTransactionAsync(string transactionId, string expand = null)
        {
            if (transactionId == null) throw new ArgumentNullException(nameof(transactionId));

            string body = await _httpClient.GetStringAsync($"transactions/{transactionId}?expand[]={expand}");
            return JsonConvert.DeserializeObject<RetrieveTransactionResponse>(body).Transaction;
        }

        public async Task<IList<Transaction>> ListTransactionsAsync(string accountId)
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));

            string body = await _httpClient.GetStringAsync($"transactions?account_id={accountId}");
            return JsonConvert.DeserializeObject<ListTransactionsResponse>(body).Transactions;
        }

        public async Task<Transaction> AnnotateTransactionAsync(string transactionId, IDictionary<string, string> metadata)
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

            HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }

            return JsonConvert.DeserializeObject<AnnotateTransactionResponse>(body).Transaction;
        }

        public async Task CreateFeedItemAsync(string accountId, string type, string url, IDictionary<string, string> @params)
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (url == null) throw new ArgumentNullException(nameof(url));
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

            HttpResponseMessage response = await _httpClient.PostAsync("feed", new FormUrlEncodedContent(formValues));
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }
        }

        public async Task<Webhook> RegisterWebhookAsync(string accountId, string url)
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));
            if (url == null) throw new ArgumentNullException(nameof(url));

            var formValues = new Dictionary<string, string>
            {
                {"account_id", accountId},
                {"url", url}
            };

            HttpResponseMessage response = await _httpClient.PostAsync("webhooks", new FormUrlEncodedContent(formValues));
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }

            return JsonConvert.DeserializeObject<RegisterWebhookResponse>(body).Webhook;
        }

        public async Task<IList<Webhook>> ListWebhooksAsync(string accountId)
        {
            if (accountId == null) throw new ArgumentNullException(nameof(accountId));

            string body = await _httpClient.GetStringAsync($"webhooks?account_id={accountId}");
            return JsonConvert.DeserializeObject<ListWebhooksResponse>(body).Webhooks;
        }

        public async Task DeleteWebhookAsync(string webhookId)
        {
            if (webhookId == null) throw new ArgumentNullException(nameof(webhookId));

            await _httpClient.DeleteAsync($"webhooks/{webhookId}");
        }

        public async Task<UploadAttachmentResponse> UploadAttachmentAsync(string filename, string fileType)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));

            var formValues = new Dictionary<string, string>
            {
                {"file_name", filename},
                {"file_type", fileType}
            };

            HttpResponseMessage response = await _httpClient.PostAsync("attachment/upload", new FormUrlEncodedContent(formValues));
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }

            return JsonConvert.DeserializeObject<UploadAttachmentResponse>(body);
        }

        public async Task<Attachment> RegisterAttachmentAsync(string externalId, string fileUrl, string fileType)
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

            HttpResponseMessage response = await _httpClient.PostAsync("attachment/register", new FormUrlEncodedContent(formValues));
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }

            return JsonConvert.DeserializeObject<RegisterAttachmentResponse>(body).Attachment;
        }

        public async Task DeregisterAttachmentAsync(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var formValues = new Dictionary<string, string>
            {
                {"id", id}
            };

            HttpResponseMessage response = await _httpClient.PostAsync("attachment/deregister", new FormUrlEncodedContent(formValues));
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
