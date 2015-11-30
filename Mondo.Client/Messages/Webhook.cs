using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class Webhook
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}