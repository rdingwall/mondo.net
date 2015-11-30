using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mondo.Client.Messages;
using Newtonsoft.Json;

namespace Mondo.Client
{
    /// <summary>
    /// Authenticated Mondo API client.
    /// </summary>
    public sealed class MondoApiClient : IMondoApiClient
    {
        private readonly HttpClient _httpClient;

        public MondoApiClient(HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            _httpClient = httpClient;
        }

        public async Task<IList<Account>> ListAccountsAsync()
        {
            string body = await _httpClient.GetStringAsync("accounts");
            return JsonConvert.DeserializeObject<ListAccountsResponse>(body).Accounts;
        }

        public async Task<Transaction> RetrieveTransactionAsync(string transactionId)
        {
            if (transactionId == null) throw new ArgumentNullException(nameof(transactionId));

            string body = await _httpClient.GetStringAsync($"transactions/{transactionId}");
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

            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            string body = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
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

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("feed", new FormUrlEncodedContent(formValues));
            string body = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
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

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("webhooks", new FormUrlEncodedContent(formValues));
            string body = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
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

            await _httpClient.DeleteAsync($"webhooks?/{webhookId}");
        }

        public Task<UploadAttachmentResponse> UploadAttachmentAsync(string filename, string fileType)
        {
            throw new NotImplementedException();
        }

        public Task<Attachment> RegisterAttachmentAsync(string externalId, string fileUrl, string fileType)
        {
            throw new NotImplementedException();
        }

        public Task DeregisterAttachmentAsync(string id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
