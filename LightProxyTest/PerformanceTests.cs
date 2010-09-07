using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using LightProxy;
using NUnit.Framework;

namespace LightProxyTest
{
    class Time : IDisposable
    {
        private readonly DateTime startTime;
        private readonly long memoryUsed;

        private Time(DateTime startTime, long memoryUsed)
        {
            this.startTime = startTime;
            this.memoryUsed = memoryUsed;
        }

        public static Time This
        {
            get { return new Time(DateTime.Now, Process.GetCurrentProcess().VirtualMemorySize64); }
        }

        public void Dispose()
        {
            Console.WriteLine("Time taken/ExtraMemory: {0} {1}", DateTime.Now - startTime, Process.GetCurrentProcess().VirtualMemorySize64 - memoryUsed);
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
            generator.GenerateProxy<IFoo>(new Blah());

            using (Time.This)
            {
                for (int i = 0; i < 1000000L; i++)
                    generator.GenerateProxy<IFoo>(new Blah(), new DoNothingInterceptor());
                GC.Collect();
            }

            var lightProxyAssembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => Regex.IsMatch(assembly.GetName().Name, "LightProxy-.*"));
            lightProxyAssembly.GetModules().SelectMany(mod => mod.GetTypes()).ShouldBeOfSize(1);            
        }
    }

    public class DoNothingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Continue();
        }
    }
}
