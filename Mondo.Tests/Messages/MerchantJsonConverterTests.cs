using Mondo.Messages;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mondo.Tests.Messages
{
    [TestFixture]
    public sealed class MerchantJsonConverterTests
    {
        [Test]
        public void Null()
        {
            var message = JsonConvert.DeserializeObject<TestMessage>("{'merchant': null}");
            Assert.IsNull(message.Merchant);
        }

        [Test]
        public void Missing()
        {
            var message = JsonConvert.DeserializeObject<TestMessage>("{}");
            Assert.IsNull(message.Merchant);
        }

        [Test]
        public void String()
        {
            var message = JsonConvert.DeserializeObject<TestMessage>("{'merchant': '1234'}");
            Assert.AreEqual("1234", message.Merchant.Id);
        }

        [Test]
        public void Object()
        {
            var message = JsonConvert.DeserializeObject<TestMessage>("{'merchant': {'id':'1234', 'name':'testMerchant'}}");
            Assert.AreEqual("1234", message.Merchant.Id);
            Assert.AreEqual("testMerchant", message.Merchant.Name);
        }

        private sealed class TestMessage
        {
            [JsonProperty("merchant")]
            [JsonConverter(typeof(MerchantJsonConverter))]
            public Merchant Merchant { get; set; }
        }
    }
}
