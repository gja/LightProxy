using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using LightProxy.Internal;

namespace LightProxy
{
    public class ProxyGenerator
    {
        private AssemblyBuilder assembly;
        private readonly Dictionary<Type, MethodInfo[]> generatedClasses = new Dictionary<Type, MethodInfo[]>();

        public T GenerateProxy<T>(T backingObject, params IInterceptor[] interceptors) where T:class
        {
            return GenerateProxy(typeof(T), backingObject, interceptors) as T;
        }

        public object GenerateProxy(Type type, object backingObject, params IInterceptor[] interceptors)
        {
            if (!generatedClasses.ContainsKey(type))
            {
                var methods = type.GetAllMethods();
                GenerateClassDynamically(type, methods);
                generatedClasses.Add(type, methods);
            }

            return GetInstance(Assembly, type, backingObject, interceptors, generatedClasses[type]);
        }

        private void GenerateClassDynamically(Type type, MethodInfo[] methods)
        {
            using (var builder = new ProxyBuilder(Assembly, type))
            {
                builder.GenerateConstructor();

                for (var i = 0; i < methods.Length; i++)
                    builder.OverrideMethod(methods[i], i);
            }
        }

        private static object GetInstance(AssemblyBuilder assembly, Type type, object backingObject, IInterceptor[] interceptors, MethodInfo[] methods)
        {
            var instance = assembly.CreateInstance(type.Name);
            var proxyBase = (ProxyBase) instance;
            proxyBase.InitializeProxy(backingObject, interceptors, methods);
            return instance;
        }

        private static AssemblyBuilder GetNewAssembly()
        {
            var assemblyName = new AssemblyName {Name = "LightProxy-" + Guid.NewGuid()};
            var assembly = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            assembly.DefineDynamicModule("Proxies");
            return assembly;
        }

        private AssemblyBuilder Assembly
        {
            get { return assembly ?? (assembly = GetNewAssembly()); }
        }
    }
}
