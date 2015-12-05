using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mondo.Messages
{
    internal sealed class ListAccountsResponse
    {
        [JsonProperty("accounts")]
        public IList<Account> Accounts { get; set; }
    }
}