using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    public interface IFoo
    {
        int Foo();
        int Bar();
        int War(object a);
        int Baz(int b, int c);
    }

    public class Blah : IFoo
    {
        public int Foo() { return 42; }
        public int Bar() { return 24; }
        public int War(object a) { return 12; }
        public int Baz(int b, int c) { return b + c; }
    }


    [TestFixture]
    public class ProxyGeneratorTest
    {
        private ProxyGenerator generator = new ProxyGenerator();
       
        [Test]
        public void ShouldGenerateProxyWithBackingObject()
        {
            var blah = generator.GenerateProxy<IFoo>(new Blah());
            blah.Foo().ShouldBe(42);
        }

        [Test]
        public void ShouldOverrideAllMembers()
        {
            var blah = generator.GenerateProxy<IFoo>(new Blah());
            blah.Bar().ShouldBe(24);
        }

        [Test]
        public void ShouldAcceptAnArgument()
        {
            var blah = generator.GenerateProxy<IFoo>(new Blah());
            blah.War(new object()).ShouldBe(12);
        }

        [Test]
        public void ShouldAcceptArgumentsIntoMethods()
        {
            var blah = generator.GenerateProxy<IFoo>(new Blah());
            blah.Baz(1, 2).ShouldBe(3); 
        }

        [Test]
        public void ShouldStoreBackingObjectAndInterceptors()
        {
            var backingObject = new Blah();
            var blah = generator.GenerateProxy<IFoo>(backingObject);

            var backing = blah.GetType().GetField("backingObject");
            backing.GetValue(blah).ShouldBe(backingObject);
        }

        [Test]
        public void ShouldNotEmitMultipleAssemblies()
        {
            generator.GenerateProxy<IFoo>(new Blah());
            generator.GenerateProxy<IFoo>(new Blah());

            AppDomain.CurrentDomain.GetAssemblies().Where(assembly => Regex.IsMatch(assembly.GetName().Name, "LightProxy-.*")).ShouldBeOfSize(1);
        }
    }
}
