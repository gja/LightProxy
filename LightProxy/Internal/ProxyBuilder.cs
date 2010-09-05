using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LightProxy.Internal
{
    internal class ProxyBuilder<T> : IDisposable
    {
        private readonly AssemblyBuilder assembly;
        private readonly ModuleBuilder module;
        private TypeBuilder newType;
        
        private MethodInfo executeMethod;

        public ProxyBuilder(AssemblyBuilder assembly)
        {
            this.assembly = assembly;
            module = assembly.GetDynamicModule("Proxies");

            var parent = typeof (ProxyBase<T>);

            newType = module.DefineType(typeof(T).Name, TypeAttributes.Public, parent, new[] { typeof(T) });
            executeMethod = parent.GetMethod("Execute");
        }

        public void GenerateConstructor()
        {
            newType.DefineDefaultConstructor(MethodAttributes.Public);
        }

        public void OverrideMethod(MethodInfo method)
        {
            var parameters = method.GetParameters().Select(param => param.ParameterType).ToArray();
            var count = parameters.Count();

            var newMethod = newType.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, parameters);

            var generator = newMethod.GetILGenerator();
            generator.DeclareLocal(typeof (object[]));
            
            generator.LoadSelf();
            
            generator.Execute(typeof(MethodBase).GetMethod("GetCurrentMethod"));

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
  
            generator.Execute(executeMethod);

            generator.Return(method.ReturnType);
                        
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