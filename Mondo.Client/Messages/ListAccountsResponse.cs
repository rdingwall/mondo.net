using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class ListAccountsResponse
    {
        [JsonProperty("accounts")]
        public IList<Account> Accounts { get; set; }
    }
}