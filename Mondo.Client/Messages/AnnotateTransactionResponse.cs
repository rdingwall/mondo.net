using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class AnnotateTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}