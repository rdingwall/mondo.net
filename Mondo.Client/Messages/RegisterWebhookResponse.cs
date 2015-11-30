using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class RegisterWebhookResponse
    {
        [JsonProperty("webhook")]
        public Webhook Webhook { get; set; }
    }
}