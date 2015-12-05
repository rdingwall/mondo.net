using System;
using NUnit.Framework;
using Mondo.Extensions;

namespace Mondo.Tests.Extensions
{
    [TestFixture]
    public sealed class DateTimeExtensionsTests
    {
        [Test]
        public void ToRfc3339String()
        {
            Assert.AreEqual("2015-04-05T18:01:32Z", new DateTime(2015, 4, 5, 18, 1, 32, DateTimeKind.Utc).ToRfc3339String());
        }
    }
}
