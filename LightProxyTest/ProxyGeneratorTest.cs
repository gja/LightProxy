using System;
using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    public interface IFoo
    {
        int Foo();
        int Bar();
        int Baz(int b, int c);
    }

    public class Blah : IFoo
    {
        public int Foo() { return 42; }
        public int Bar() { return 24; }
        public int Baz(int b, int c) { return b + c; }
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

        [Test]
        public void ShouldAcceptArgumentsIntoMethods()
        {
            var blah = new ProxyGenerator().GenerateProxy<IFoo>(new Blah());
            blah.Baz(1, 2).ShouldBe(3);
        }

        [Test]
        public void ShouldStoreBackingObjectAndInterceptors()
        {
            var backingObject = new Blah();
            var blah = new ProxyGenerator().GenerateProxy<IFoo>(backingObject);

            var backing = blah.GetType().GetField("backingObject");
            backing.GetValue(blah).ShouldBe(backingObject);
        }
    }
}
