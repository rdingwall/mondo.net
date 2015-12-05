using Newtonsoft.Json;

namespace Mondo.Messages
{
    internal sealed class RetrieveTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}