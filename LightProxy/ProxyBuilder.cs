using System;
using System.Reflection;
using System.Reflection.Emit;

namespace LightProxy
{
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
            
            newType = module.DefineType(typeof(T).Name, TypeAttributes.Public, typeof(Object), new[] { typeof(T), typeof(ISetBackingObjectAndInterceptors<T>) });
            backingObjectField = newType.DefineField("backingObject", typeof (T), FieldAttributes.Public);
            interceptorsField = newType.DefineField("interceptors", typeof (T), FieldAttributes.Public);            
        }

        public void GenerateConstructor()
        {
            newType.DefineDefaultConstructor(MethodAttributes.Public);
        }


        public void GenerateSetterMethod()
        {
            var method = newType.DefineMethod("__LightProxy_SetMethods", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), new[] {typeof (T), typeof (IInterceptor[])});
            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, backingObjectField);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Stfld, interceptorsField);

            generator.Emit(OpCodes.Ret);

            newType.DefineMethodOverride(method, typeof(ISetBackingObjectAndInterceptors<T>).GetMethods()[0]);
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