using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    public interface IFoo
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
            var foo = blah.Foo();
            foo.ShouldBe(42);
        }
    }
}
