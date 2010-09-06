using System;
using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    class Time : IDisposable
    {
        private readonly DateTime startTime;

        private Time(DateTime startTime)
        {
            this.startTime = startTime;
        }

        public static Time This
        {
            get { return new Time(DateTime.Now); }
        }

        public void Dispose()
        {
            Console.WriteLine("Time taken: {0}", DateTime.Now - startTime);
        }
    }

    [TestFixture, Ignore]
    public class PerformanceTests : ProxyGeneratorTestBase
    {
        [Test]
        public void ShouldBeReallyReallyFast()
        {
            IFoo original = new Blah();

            var foo = original;
            using (Time.This)
                for (int i = 0; i < 1000000L; i++) { foo.Junk(); }

            foo = generator.GenerateProxy(foo);
            using (Time.This)
                for (int i = 0; i < 1000000L; i++) { foo.Junk(); }

            foo = generator.GenerateProxy(foo, new DoNothingInterceptor());
            using (Time.This)
                for (int i = 0; i < 1000000L; i++) { foo.Junk(); }

            foo = generator.GenerateProxy(foo, new DoNothingInterceptor(), new DoNothingInterceptor());
            using (Time.This)
                for (int i = 0; i < 1000000L; i++) { foo.Junk(); }
        }
    }

    public class DoNothingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Continue(); ;
        }
    }
}
