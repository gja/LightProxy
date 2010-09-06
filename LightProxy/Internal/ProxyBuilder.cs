using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LightProxy.Internal
{
    internal class ProxyBuilder<T> : IDisposable
    {
        private readonly ModuleBuilder module;
        private readonly TypeBuilder newType;
        
        private readonly MethodInfo executeMethod;
        private FieldInfo backingObject;

        public ProxyBuilder(AssemblyBuilder assembly)
        {
            module = assembly.GetDynamicModule("Proxies");

            var parent = typeof (ProxyBase<T>);

            newType = module.DefineType(typeof(T).Name, TypeAttributes.Public, parent, new[] { typeof(T) });
            executeMethod = parent.GetMethod("Execute");
            backingObject = parent.GetField("backingObject");
        }

        public void GenerateConstructor()
        {
            newType.DefineDefaultConstructor(MethodAttributes.Public);
        }

        public void OverrideMethod(MethodInfo method, int position)
        {
            var parameters = method.GetParameters().Select(param => param.ParameterType).ToArray();
            var count = parameters.Count();

            var continueMethod = newType.DefineMethod("__LightProxy_Continue_" + method.Name + position, MethodAttributes.Public, method.CallingConvention, typeof(object), new[] {typeof(object[])});
            CreateContinueMethod(method, continueMethod, position, parameters, count);

            var newMethod = newType.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, parameters);
            GenerateProxyMethod(method, newMethod, position, parameters, count, continueMethod);

            newType.DefineMethodOverride(newMethod, method);
        }

        private void GenerateProxyMethod(MethodInfo method, MethodBuilder newMethod, int position, Type[] parameters, int count, MethodBuilder continueMethod)
        {
            var generator = newMethod.GetILGenerator();
            generator.DeclareLocal(typeof (object[]));
            
            generator.LoadSelf();
            
            // Pass position of the method in the method array
            generator.LoadInteger(position);

            generator.CreateArray(typeof (Object), count);            
            generator.StashTemporaryVariable(0);
            for (int i = 0; i < count; i++)
            {
                generator.LoadTemporaryVariable(0);
                generator.LoadInteger(i);
                generator.LoadArgument(i + 1);
                generator.MaybeBox(parameters[i]);
                generator.Emit(OpCodes.Stelem_Ref);
            }
            generator.LoadTemporaryVariable(0);

            var ctor = typeof (Func<object[], object>).GetConstructor(new[] {typeof (object), typeof (IntPtr)});

            generator.LoadSelf();
            generator.Emit(OpCodes.Ldftn, continueMethod);
            generator.Emit(OpCodes.Newobj, ctor);

            generator.Execute(executeMethod);

            generator.Return(method.ReturnType);
        }

        private void CreateContinueMethod(MethodInfo method, MethodBuilder continueMethod, int position, Type[] parameters, int count)
        {
            var generator = continueMethod.GetILGenerator();
            generator.DeclareLocal(typeof (object[]));
            generator.LoadArgument(1);
            generator.StashTemporaryVariable(0);

            generator.LoadField(backingObject);

            for (int i = 0; i < count; i++)
            {
                generator.LoadTemporaryVariable(0);
                generator.LoadInteger(i);
                generator.Emit(OpCodes.Ldelem_Ref);
                generator.MaybeUnBox(parameters[i]);                
            }

            generator.Execute(method);

            if (method.ReturnType == typeof(void))
                generator.LoadNull();
            else
                generator.MaybeBox(method.ReturnType);

            generator.Emit(OpCodes.Ret);
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