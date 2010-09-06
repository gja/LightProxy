using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        void Junk();
        int Foo(int val);
        string Name { get; set; }
    }

    public class Blah : IFoo
    {
        public int Foo() { return 42; }
        public int Foo(int val) { return 2 * val; }
        public string Name { get; set; }
        public int Bar() { return 24; }
        public int War(object a) { return 12; }
        public int Baz(int b, int c) { return b + c; }
        public void Junk() { }
    }

    [TestFixture]
    public class ProxyGeneratorTest : ProxyGeneratorTestBase
    {
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

        [Test]
        public void ShouldBeAbleToInterceptAVoidMethod()
        {
            var blah = generator.GenerateProxy<IFoo>(new Blah());
            blah.Junk();
        }

        [Test]
        public void ShouldBeAbleToInterceptTwoMethodsWitheTheSameName()
        {
            var blah = generator.GenerateProxy<IFoo>(new Blah());
            blah.Foo().ShouldBe(42);
            blah.Foo(5).ShouldBe(10);
        }

        [Test]
        public void ShouldBeAbleToInterceptProperties()
        {
            var list = new List<int>();
            var blah = generator.GenerateProxy<IFoo>(new Blah(), new TailInterceptor(list));
            blah.Name = "Braindead";
            blah.Name.ShouldBe("Braindead");
            list.ShouldBe(new List<int>{0,4,0,4});
        }

        [Test]
        public void ShouldPutItAllTogether()
        {
            var list = new List<int>();
            var blah = generator.GenerateProxy<IFace>(new InterceptedFace(list), new IInterceptor[] {new TailInterceptor(list), new FaceInterceptor(list)});
            
            blah.Foo().ShouldBe(10);

            list.ShouldBe(new List<int>{0,1,2,3,4});
        }
    }
}
