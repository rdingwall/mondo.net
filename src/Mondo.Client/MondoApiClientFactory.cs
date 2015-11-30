using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Mondo.Client.Messages;
using Newtonsoft.Json;

namespace Mondo.Client
{
    public sealed class MondoApiClientFactory : IMondoApiClientFactory
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly Uri _url;

        public MondoApiClientFactory(string url, string clientId, string clientSecret)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            if (clientSecret == null) throw new ArgumentNullException(nameof(clientSecret));

            _clientId = clientId;
            _clientSecret = clientSecret;
            _url = new Uri(url);
        }

        public async Task<IMondoApiClient> Authenticate(string username, string password)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            if (password == null) throw new ArgumentNullException(nameof(password));

            var formValues = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", _clientId},
                {"client_secret", _clientSecret},
                {"username", username},
                {"password", password}
            };

            var httpClient = new HttpClient {BaseAddress = _url};

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("oauth2/token", new FormUrlEncodedContent(formValues));
            string body = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new MondoApiException(body);
            }
                        
            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(body);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResponse.AccessToken);
            return new MondoApiClient(httpClient);
        }
    }
}