using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace LightProxy
{
    public class ProxyGenerator
    {
        public T GenerateProxy<T>(T backingObject, params IInterceptor[] interceptors) where T:class 
        {
            var assembly = GetNewAssembly();

            using (var builder = new ProxyBuilder<T>(assembly))
            {
                builder.GenerateConstructor();

                builder.GenerateSetterMethod();

                foreach (var method in typeof(T).GetMethods())
                    builder.OverrideMethod(method);
            }

            return GetInstance(assembly, typeof(T), backingObject, interceptors);
        }

        private T GetInstance<T>(AssemblyBuilder assembly, Type type, T backingObject, IInterceptor[] interceptors)
        {
            var instance = assembly.CreateInstance(type.Name);
            var proxyBase = (ISetBackingObjectAndInterceptors<T>) instance;
            proxyBase.SetBackingObjectAndInterceptors(backingObject, interceptors);
            return (T) instance;
        }

        private AssemblyBuilder GetNewAssembly()
        {
            var assemblyName = new AssemblyName();
            assemblyName.Name = "LightProxy" + Guid.NewGuid();
            return Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        }
    }
}
