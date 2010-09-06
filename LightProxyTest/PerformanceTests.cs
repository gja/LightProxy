using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        [Test]
        public void ShouldNotTakeMoreThan1kbToCreateOneMillionObjects()
        {
            long totalSize;
            generator.GenerateProxy<IFoo>(new Blah());

            var lightProxyAssembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => Regex.IsMatch(assembly.GetName().Name, "LightProxy-.*"));

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            totalSize= GC.GetTotalMemory(true);
            for (int i = 0; i < 1000000L; i++)
                generator.GenerateProxy<IFoo>(new Blah());
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.GetTotalMemory(true).ShouldBe(totalSize, 1000);

            lightProxyAssembly.GetModules().SelectMany(mod => mod.GetTypes()).ShouldBeOfSize(1);            
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
