using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class RegisterAttachmentResponse
    {
        [JsonProperty("attachment")]
        public Attachment Attachment { get; set; }
    }
}