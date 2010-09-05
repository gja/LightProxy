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

//            GenerateConstructor<T>(newType);
            newType.DefineDefaultConstructor(MethodAttributes.Public);

            var methods = typeof (T).GetMethods();

            foreach(var method in methods)
            {
                var newMethod = newType.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, method.GetGenericArguments());
                var generator = newMethod.GetILGenerator();

                generator.Emit(OpCodes.Ldc_I4, 42);
                generator.Emit(OpCodes.Ret);

                newType.DefineMethodOverride(newMethod, method);
            }

            var type = newType.CreateType();

            return GetInstance(newAssembly, type, backingObject, interceptors);
        }

        private T GetInstance<T>(AssemblyBuilder assembly, Type type, T backingObject, IInterceptor[] interceptors)
        {
            var instance = assembly.CreateInstance(type.Name);
            var proxyBase = (ProxyBase<T>) instance;
            proxyBase.BackingObject = backingObject;
            proxyBase.Interceptors = interceptors;
            return (T) instance;
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
            
            var constructor = newType.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, constructorTypes);

            var generator = constructor.GetILGenerator();
            
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, newType.BaseType.GetConstructor(constructorTypes));
            generator.Emit(OpCodes.Ret);
        }
    }
}
