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
            var newAssembly = GetNewAssembly();

            TypeBuilder newType = GetNewType<T>(newAssembly);

            GenerateConstructor<T>(newType);

            var methods = typeof (T).GetMethods();

            foreach(var method in methods)
            {
                var newMethod = newType.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, method.GetGenericArguments());
                var generator = newMethod.GetILGenerator();

                generator.Emit(OpCodes.Ldc_I4, 42);
                generator.Emit(OpCodes.Ret);

                newType.DefineMethodOverride(newMethod, method);
            }

            newType.CreateType();

            return (T) newAssembly.CreateInstance(newType.Name, false, BindingFlags.Public, null, new object[]{backingObject, interceptors}, null, null);
        }

        private TypeBuilder GetNewType<T>(AssemblyBuilder newAssembly)
        {
            var newModule = newAssembly.DefineDynamicModule("Proxies");

            return newModule.DefineType(typeof(T).Name + "Proxy", TypeAttributes.Public, typeof(ProxyBase<T>), new[]{typeof(T)});
        }

        private AssemblyBuilder GetNewAssembly()
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "LightProxy" + Guid.NewGuid();
            
            //Create an assembly with one module
            return Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        }

        private void GenerateConstructor<T>(TypeBuilder newType)
        {
            var constructorTypes = new[] {typeof (T), typeof (IInterceptor[])};
            var constructor = newType.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, constructorTypes).GetILGenerator();
            constructor.Emit(OpCodes.Ldarg_0);
            constructor.Emit(OpCodes.Ldarg_1);
            constructor.Emit(OpCodes.Call, newType.BaseType.GetConstructor(constructorTypes));
            constructor.Emit(OpCodes.Ret);
        }
    }
}
