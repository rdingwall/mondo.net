using Newtonsoft.Json;

namespace Monzo.Messages
{
    internal sealed class RegisterWebhookResponse
    {
        [JsonProperty("webhook")]
        public Webhook Webhook { get; set; }
    }
}