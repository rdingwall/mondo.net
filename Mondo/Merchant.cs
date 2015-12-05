using System;
using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// The merchant a transaction wasd made at.
    /// </summary>
    public sealed class Merchant
    {
        /// <summary>
        /// The address of the merchant.
        /// </summary>
        [JsonProperty("address")]
        public MerchantAddress Address { get; set; }

        /// <summary>
        /// The time the merchant was created at.
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Used to group individual merchants who are part of a chain.
        /// </summary>
        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// The merchant's Id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The merchant's logo image URL.
        /// </summary>
        [JsonProperty("logo")]
        public string Logo { get; set; }

        /// <summary>
        /// The emoji displayed for the merchant.
        /// </summary>
        [JsonProperty("emoji")]
        public string Emoji { get; set; }

        /// <summary>
        /// The name of the merchant.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The category of the merchant.
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }
    }
}