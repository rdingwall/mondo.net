using System.Collections.Generic;
using Newtonsoft.Json;

namespace Monzo.Messages
{
    internal sealed class ListWebhooksResponse
    {
        [JsonProperty("webhooks")]
        public IList<Webhook> Webhooks { get; set; }
    }
}