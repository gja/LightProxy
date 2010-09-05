using System;
using System.Diagnostics.SymbolStore;
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
            var count = parameters.Count();

            var newMethod = newType.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, parameters);

            var generator = newMethod.GetILGenerator();
            generator.DeclareLocal(typeof (object[]));

            generator.LoadSelf();
            
            generator.LoadNull();
            generator.LoadField(backingObjectField);
            generator.LoadField(interceptorsField);

            generator.CreateArray(typeof (Object), count);            
            generator.StashTemporaryVariable(0);
            for (int i = 0; i < count; i++)
            {
                generator.LoadTemporaryVariable(0);
                generator.LoadInteger(i);
                generator.LoadArgument(i + 1);
                
                var type = parameters[i];
                if(type.IsValueType)
                    generator.Box(type);
                
                generator.Emit(OpCodes.Stelem_Ref);
            }
            generator.LoadTemporaryVariable(0);
  
            generator.Execute(executeMethod);

            generator.Return();
                        
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

        private void ThrowException(ILGenerator generator)
        {
            generator.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new Type[0]));
            generator.Emit(OpCodes.Throw);
        }
    }
}