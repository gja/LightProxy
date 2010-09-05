using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace LightProxyTest
{
    public static class Should
    {
        public static void ShouldBe(this object actual, object expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

        public static void ShouldBeOfSize<T>(this IEnumerable<T> actual, int expected)
        {
            actual.Count().ShouldBe(expected);
        }
    }
}
