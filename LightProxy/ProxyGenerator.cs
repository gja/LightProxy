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

                foreach (var method in typeof(T).GetMethods())
                    builder.OverrideMethod(method);
            }

            return GetInstance(assembly, typeof(T), backingObject, interceptors);
        }

        private T GetInstance<T>(AssemblyBuilder assembly, Type type, T backingObject, IInterceptor[] interceptors)
        {
            var instance = assembly.CreateInstance(type.Name);
//            var proxyBase = (ProxyBase<T>) instance;
//            proxyBase.BackingObject = backingObject;
//            proxyBase.Interceptors = interceptors;
            return (T) instance;
        }

        private AssemblyBuilder GetNewAssembly()
        {
            var assemblyName = new AssemblyName();
            assemblyName.Name = "LightProxy" + Guid.NewGuid();
            return Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        }
    }

    public class ProxyBuilder<T> : IDisposable
    {
        private readonly AssemblyBuilder assembly;
        private readonly ModuleBuilder module;
        private TypeBuilder newType;
        private FieldBuilder backingObjectField;
        private FieldBuilder interceptorsField;

        public ProxyBuilder(AssemblyBuilder assembly)
        {
            this.assembly = assembly;
            module = assembly.DefineDynamicModule(typeof(T).Name + "Proxy");
            
            newType = module.DefineType(typeof(T).Name, TypeAttributes.Public, typeof(Object), new[] { typeof(T) });
            backingObjectField = newType.DefineField("backingObject", typeof (T), FieldAttributes.Family);
            interceptorsField = newType.DefineField("interceptors", typeof (T), FieldAttributes.Family);            
        }

        public void GenerateConstructor()
        {
            newType.DefineDefaultConstructor(MethodAttributes.Public);
        }

        public void OverrideMethod(MethodInfo method)
        {
            var newMethod = newType.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, method.GetGenericArguments());

            var generator = newMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldc_I4, 42);
            generator.Emit(OpCodes.Ret);

            newType.DefineMethodOverride(newMethod, method);
        }

        public void Build()
        {
            newType.CreateType();
        }

        public void Dispose()
        {
            Build();
        }
    }
}
