using System.Collections.Generic;
using Newtonsoft.Json;

namespace Monzo.Messages
{
    internal sealed class ListAccountsResponse
    {
        [JsonProperty("accounts")]
        public IList<Account> Accounts { get; set; }
    }
}