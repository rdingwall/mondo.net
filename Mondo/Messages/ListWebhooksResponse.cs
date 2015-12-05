using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mondo.Messages
{
    internal sealed class ListWebhooksResponse
    {
        [JsonProperty("webhooks")]
        public IList<Webhook> Webhooks { get; set; }
    }
}