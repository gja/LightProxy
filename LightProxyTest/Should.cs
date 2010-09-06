using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace LightProxyTest
{
    public static class Should
    {
        public static void ShouldBe(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldBeOfSize<T>(this IEnumerable<T> actual, int expected)
        {
            actual.Count().ShouldBe(expected);
        }

        public static void ShouldBe(this long actual, long expected, long tolerance)
        {
            Assert.AreEqual(expected, actual, tolerance);
        }
    }
}
