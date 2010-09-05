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
        public List<Type> generatedClasses = new List<Type>();

        public T GenerateProxy<T>(T backingObject, params IInterceptor[] interceptors) where T:class
        {
            if (!generatedClasses.Contains(typeof(T)))
            {
                GenerateClassDynamically<T>();
                generatedClasses.Add(typeof (T));
            }

            return GetInstance(Assembly, typeof(T), backingObject, interceptors);
        }

        private void GenerateClassDynamically<T>()
        {
            using (var builder = new ProxyBuilder<T>(Assembly))
            {
                builder.GenerateConstructor();

                foreach (var method in typeof(T).GetMethods())
                    builder.OverrideMethod(method);
            }
        }

        private T GetInstance<T>(AssemblyBuilder assembly, Type type, T backingObject, IInterceptor[] interceptors)
        {
            var instance = assembly.CreateInstance(type.Name);
            var proxyBase = (ProxyBase<T>) instance;
            proxyBase.SetBackingObjectAndInterceptors(backingObject, interceptors);
            return (T) instance;
        }

        private AssemblyBuilder GetNewAssembly()
        {
            var assemblyName = new AssemblyName();
            assemblyName.Name = "LightProxy-" + Guid.NewGuid();
            var assembly = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            assembly.DefineDynamicModule("Proxies");
            return assembly;
        }

        public AssemblyBuilder Assembly
        {
            get { return assembly ?? (assembly = GetNewAssembly()); }
        }
    }
}
