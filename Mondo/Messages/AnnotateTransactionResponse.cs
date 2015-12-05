using Newtonsoft.Json;

namespace Mondo.Messages
{
    public sealed class AnnotateTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}