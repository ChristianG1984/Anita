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

        [Test]
        public void
        ensure_that_the_calculator_works() {
            var result = Dummy.add(2, 2);
            Assert.That(result, Is.EqualTo(4));
        }
    }
}
