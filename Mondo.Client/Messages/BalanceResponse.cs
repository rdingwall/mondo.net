using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class BalanceResponse
    {
        [JsonProperty("balance")]
        public long Balance { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("spend_today")]
        public long SpendToday { get; set; }
    }
}
