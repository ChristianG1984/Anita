using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anita.Api.Tests
{
    [TestFixture]
    public class MyTestShould
    {
        [Test]
        public void
        ensure_that_1_plus_1_is_still_2() {
            Assert.That(1 + 1, Is.EqualTo(2));
        }
    }
}
