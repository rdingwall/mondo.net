using Newtonsoft.Json;

namespace Monzo.Messages
{
    internal sealed class RegisterAttachmentResponse
    {
        [JsonProperty("attachment")]
        public Attachment Attachment { get; set; }
    }
}