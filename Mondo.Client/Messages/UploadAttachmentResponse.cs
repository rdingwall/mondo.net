using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class UploadAttachmentResponse
    {
        /// <summary>
        /// The URL of the file once it has been uploaded
        /// </summary>
        [JsonProperty("file_url")]
        public string FileUrl { get; set; }

        /// <summary>
        /// The URL to POST the file to when uploading
        /// </summary>
        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }
    }
}