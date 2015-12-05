using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mondo.Messages
{
    internal sealed class MerchantJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.ToObject<Merchant>();

                case JTokenType.String:
                    return new Merchant {Id = token.ToString()};

                default:
                    return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Merchant);
        }
    }
}