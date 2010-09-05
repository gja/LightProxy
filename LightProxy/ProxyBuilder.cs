using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LightProxy
{
    class ProxyBuilder<T> : IDisposable
    {
        private readonly AssemblyBuilder assembly;
        private readonly ModuleBuilder module;
        private TypeBuilder newType;
        private FieldInfo backingObjectField;
        private FieldInfo interceptorsField;
        private MethodInfo executeMethod;

        public ProxyBuilder(AssemblyBuilder assembly)
        {
            this.assembly = assembly;
            module = assembly.GetDynamicModule("Proxies");

            var parent = typeof (ProxyBase<T>);

            newType = module.DefineType(typeof(T).Name, TypeAttributes.Public, parent, new[] { typeof(T), typeof(ISetBackingObjectAndInterceptors<T>) });
            backingObjectField = parent.GetField("backingObject");
            interceptorsField = parent.GetField("interceptors");
            executeMethod = parent.GetMethod("Execute");
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
            var parameters = method.GetParameters().Select(param => param.ParameterType).ToArray();
            var newMethod = newType.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, parameters);

            var generator = newMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, backingObjectField);

            for (int i = 0; i < parameters.Count(); i++)
                generator.Emit(OpCodes.Ldarg, i + 1);

            generator.Emit(OpCodes.Call, method);
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