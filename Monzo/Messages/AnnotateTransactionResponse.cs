using Newtonsoft.Json;

namespace Monzo.Messages
{
    internal sealed class AnnotateTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}