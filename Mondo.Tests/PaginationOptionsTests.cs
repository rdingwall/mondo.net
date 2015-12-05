using System;
using NUnit.Framework;

namespace Mondo.Tests
{
    [TestFixture]
    public sealed class PaginationOptionsTests
    {
        [Test]
        public void SinceTime()
        {
            Assert.AreEqual("&limit=&since=2015-04-05T18:01:32Z&before=", new PaginationOptions { SinceTime = new DateTime(2015, 4, 5, 18, 1, 32, DateTimeKind.Utc) }.ToString());
        }

        [Test]
        public void BeforeTime()
        {
            Assert.AreEqual("&limit=&since=&before=2015-04-05T18:01:32Z", new PaginationOptions { BeforeTime = new DateTime(2015, 4, 5, 18, 1, 32, DateTimeKind.Utc) }.ToString());
        }

        [Test]
        public void SinceId()
        {
            Assert.AreEqual("&limit=&since=1&before=", new PaginationOptions { SinceId = "1" }.ToString());
        }

        [Test]
        public void Limit()
        {
            Assert.AreEqual("&limit=100&since=&before=", new PaginationOptions { Limit = 100 }.ToString());
        }
    }
}
