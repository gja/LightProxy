using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    internal interface IFoo
    {
        int Foo();
    }

    public class Blah : IFoo
    {
        public int Foo()
        {
            return 42;
        }
    }


    [TestFixture]
    public class ProxyGeneratorTest
    {
        [Test]
        public void ShouldGenerateProxyWithBackingObject()
        {
            var blah = new ProxyGenerator().GenerateProxy<IFoo>(new Blah());
            blah.Foo().ShouldBe(42);
        }
    }
}
