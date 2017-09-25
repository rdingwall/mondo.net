using Newtonsoft.Json;

namespace Monzo.Messages
{
    internal sealed class RetrieveTransactionResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}