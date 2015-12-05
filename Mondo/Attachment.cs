using System;
using Newtonsoft.Json;

namespace Mondo
{
    /// <summary>
    /// Images (eg. receipts) can be attached to transactions by uploading these via the attachment API. Once an attachment is registered against a transaction, the image will be shown in the transaction detail screen within the Mondo app.
    /// </summary>
    public sealed class Attachment
    {
        /// <summary>
        /// The ID of the attachment. This can be used to deregister at a later date.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The id of the user who owns this attachment.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// The id of the transaction to associate the attachment with.
        /// </summary>
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        /// <summary>
        /// The URL of the uploaded attachment.
        /// </summary>
        [JsonProperty("file_url")]
        public string FileUrl { get; set; }

        /// <summary>
        /// The content type of the attachment.
        /// </summary>
        [JsonProperty("file_type")]
        public string FileType { get; set; }

        /// <summary>
        /// The timestamp in UTC when the attachment was created.
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}