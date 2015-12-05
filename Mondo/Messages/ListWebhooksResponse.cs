using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mondo.Messages
{
    public sealed class ListWebhooksResponse
    {
        [JsonProperty("webhooks")]
        public IList<Webhook> Webhooks { get; set; }
    }
}