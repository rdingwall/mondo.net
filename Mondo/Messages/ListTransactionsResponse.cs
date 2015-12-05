using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mondo.Messages
{
    public sealed class ListTransactionsResponse
    {
        [JsonProperty("transactions")]
        public IList<Transaction> Transactions { get; set; }
    }
}