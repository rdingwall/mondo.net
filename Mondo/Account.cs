using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// Accounts represent a store of funds, and have a list of transactions.
    /// </summary>
    [DebuggerDisplay("[{Id,nq} {Description}]")]
    public sealed class Account
    {
        /// <summary>
        /// The id of the account.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The account description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// When the account was created.
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}