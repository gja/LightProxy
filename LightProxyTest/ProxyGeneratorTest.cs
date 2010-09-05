using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    public interface IFoo
    {
        int Foo();
        int Bar();
    }

    public class Blah : IFoo
    {
        public int Foo()
        {
            return 42;
        }

        public int Bar()
        {
            return 24;
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

        [Test]
        public void ShouldOverrideAllMembers()
        {
            var blah = new ProxyGenerator().GenerateProxy<IFoo>(new Blah());
            blah.Bar().ShouldBe(24);
        }
    }
}
