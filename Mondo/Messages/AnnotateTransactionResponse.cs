using Newtonsoft.Json;

namespace Mondo.Messages
{
    internal sealed class AnnotateTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}