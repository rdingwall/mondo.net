using Newtonsoft.Json;

namespace Mondo.Messages
{
    public sealed class RegisterWebhookResponse
    {
        [JsonProperty("webhook")]
        public Webhook Webhook { get; set; }
    }
}