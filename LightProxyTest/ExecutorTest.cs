using System;
using System.Collections.Generic;
using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    public interface IFace
    {
        int Foo();
        bool Called { get; }
    }

    public class Face : IFace
    {
        public bool called;
        public bool Called
        {
            get { return called; }
        }

        public int Foo() { called = true; return 10; }
    }

    [TestFixture]
    public class ExecutorTest
    {
        [Test]
        public void ShouldBeAbleToCreateAnExecutor()
        {
            var proxy = new ProxyBase<IFace>();
            proxy.backingObject = new Face();
            proxy.interceptors = new IInterceptor[0];

            var invocation = new Invocation(proxy.backingObject, proxy.interceptors, typeof(IFace).GetMethods()[0], new object[0]);
            invocation.Continue();
            invocation.ReturnValue.ShouldBe(10);
            proxy.backingObject.Called.ShouldBe(true);
        }
    }
}
