using NUnit.Framework;

namespace LightProxyTest
{
    public static class Should
    {
        public static void ShouldBe(this object actual, object expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
