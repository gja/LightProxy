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

            //            //Define a method on the new type to call
//            Type[] paramTypes = new Type[0];
//            Type returnType = typeof(long);
//            MethodBuilder newMethod = newType.DefineMethod("FindFactorial", MethodAttributes.Public | MethodAttributes.Virtual,
//                returnType, paramTypes);
//            //Get an IL Generator, which will be used to emit the required IL
//            ILGenerator generator = newMethod.GetILGenerator();
//
//            //Emit the IL in lines of the FindFactorial defined above
//            generator.Emit(OpCodes.Ldc_I8, 1L); //push '1' into the evaluation stack
//            for (long i = 1L; i <= number; i++)
//            {
//                //push the current number into evaluation stack
//                generator.Emit(OpCodes.Ldc_I8, i);
//                //muliply the two successive numbers and push into the evaluation stack
//                generator.Emit(OpCodes.Mul);
//            }
//            //return the value;
//            generator.Emit(OpCodes.Ret);
//
//            //Now override the method defination from IFactorial in the FindFactorial class
//            MethodInfo methodInfo = typeof(IFactorial).GetMethod("FindFactorial");
//            newType.DefineMethodOverride(newMethod, methodInfo);

            newType.CreateType();

            return (T) newAssembly.CreateInstance(typeof(T).Name, false, BindingFlags.Public, null, new object[]{backingObject, interceptors}, null, null);
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
