using System.Collections.Generic;
using Newtonsoft.Json;

namespace Monzo.Messages
{
    internal sealed class ListTransactionsResponse
    {
        [JsonProperty("transactions")]
        public IList<Transaction> Transactions { get; set; }
    }
}