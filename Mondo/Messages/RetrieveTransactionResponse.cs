using Newtonsoft.Json;

namespace Mondo.Messages
{
    public sealed class RetrieveTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}