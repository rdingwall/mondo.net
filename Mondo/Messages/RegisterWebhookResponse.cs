using Newtonsoft.Json;

namespace Mondo.Messages
{
    internal sealed class RegisterWebhookResponse
    {
        [JsonProperty("webhook")]
        public Webhook Webhook { get; set; }
    }
}