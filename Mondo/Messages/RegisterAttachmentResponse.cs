using Newtonsoft.Json;

namespace Mondo.Messages
{
    internal sealed class RegisterAttachmentResponse
    {
        [JsonProperty("attachment")]
        public Attachment Attachment { get; set; }
    }
}