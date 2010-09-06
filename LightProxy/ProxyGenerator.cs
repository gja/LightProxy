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
            var type = typeof(T);

            if (!generatedClasses.ContainsKey(type))
            {
                var methods = type.GetMethods();
                GenerateClassDynamically<T>(methods);                
                generatedClasses.Add(type, methods);
            }

            return GetInstance(Assembly, type, backingObject, interceptors, generatedClasses[type]);
        }

        private void GenerateClassDynamically<T>(MethodInfo[] methods)
        {
            using (var builder = new ProxyBuilder<T>(Assembly))
            {
                builder.GenerateConstructor();

                for (var i = 0; i < methods.Length; i++)
                    builder.OverrideMethod(methods[i], i);
            }
        }

        private static T GetInstance<T>(AssemblyBuilder assembly, Type type, T backingObject, IInterceptor[] interceptors, MethodInfo[] methods)
        {
            var instance = assembly.CreateInstance(type.Name);
            var proxyBase = (ProxyBase<T>) instance;
            proxyBase.InitializeProxy(backingObject, interceptors, methods);
            return (T) instance;
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
