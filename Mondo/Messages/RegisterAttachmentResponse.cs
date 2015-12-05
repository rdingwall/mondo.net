using Newtonsoft.Json;

namespace Mondo.Messages
{
    public sealed class RegisterAttachmentResponse
    {
        [JsonProperty("attachment")]
        public Attachment Attachment { get; set; }
    }
}