using System;
using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class Merchant
    {
        [JsonProperty("address")]
        public MerchantAddress Address { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("emoji")]
        public string Emoji { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}