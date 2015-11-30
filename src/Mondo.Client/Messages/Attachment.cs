using System;
using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class Attachment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("file_url")]
        public string FileUrl { get; set; }

        [JsonProperty("file_type")]
        public string FileType { get; set; }
        
        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}