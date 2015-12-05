using System;
using Newtonsoft.Json;

namespace Mondo.Messages
{
    public sealed class Account
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}