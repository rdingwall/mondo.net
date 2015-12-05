using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// The merchant's address.
    /// </summary>
    public sealed class MerchantAddress
    {
        /// <summary>
        /// The merchant's address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The merchant's city.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// The merchant's country.
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// The merchant's latitude.
        /// </summary>
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// The merchant's longitude.
        /// </summary>
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// The merchant's postcode.
        /// </summary>
        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        /// <summary>
        /// The merchant's region.
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }
    }
}