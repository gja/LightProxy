using System;
using System.Collections.Generic;
using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    public interface IFace
    {
        int Foo();
    }

    public class Face : IFace
    {
        public bool called;

        public int Foo() { called = true; return 10; }
    }

    public class InterceptedFace : IFace
    {
        private readonly List<int> list;

        public InterceptedFace(List<int> list)
        {
            this.list = list;
        }

        public int Foo()
        {
            list.Add(2);
            return 10;
        }
    }

    public class FaceInterceptor : IInterceptor
    {
        private readonly List<int> list;

        public FaceInterceptor(List<int> list)
        {
            this.list = list;
        }

        public void Intercept(IInvocation invocation)
        {
            list.Add(1);
            invocation.Continue();
            list.Add(3);
        }
    }
    
    public class TailInterceptor : IInterceptor
    {
        private readonly List<int> list;

        public TailInterceptor(List<int> list)
        {
            this.list = list;
        }

        public void Intercept(IInvocation invocation)
        {
            list.Add(0);
            invocation.Continue();
            list.Add(4);
        }
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
            (proxy.backingObject as Face).called.ShouldBe(true);
        }

        [Test]
        public void ShouldBeAbleToIntercept()
        {
            var list = new List<int>();
            var proxy = new ProxyBase<IFace>();
            proxy.backingObject = new InterceptedFace(list);
            proxy.interceptors = new IInterceptor[]{new FaceInterceptor(list)};

            var invocation = new Invocation(proxy.backingObject, proxy.interceptors, typeof(IFace).GetMethods()[0], new object[0]);
            invocation.Continue();
            invocation.ReturnValue.ShouldBe(10);
            list.ShouldBe(new List<int>{1,2,3});
        }

        [Test]
        public void ShouldBeAbleToInterceptWithMultipleInterceptors()
        {
            var list = new List<int>();
            var proxy = new ProxyBase<IFace>();
            proxy.backingObject = new InterceptedFace(list);
            proxy.interceptors = new IInterceptor[]{new FaceInterceptor(list), new TailInterceptor(list)};

            var invocation = new Invocation(proxy.backingObject, proxy.interceptors, typeof(IFace).GetMethods()[0], new object[0]);
            invocation.Continue();
            invocation.ReturnValue.ShouldBe(10);
            list.ShouldBe(new List<int>{0,1,2,3,4});
        }
    }
}
