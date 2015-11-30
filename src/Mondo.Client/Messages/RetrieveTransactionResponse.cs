using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class RetrieveTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}