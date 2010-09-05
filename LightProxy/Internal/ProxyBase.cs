using System;
using System.Linq;
using System.Reflection;

namespace LightProxy.Internal
{
    public class ProxyBase<T>
    {
        public T backingObject;
        public IInterceptor[] interceptors;
        private InterfaceMapping interfaceMap;

        public object Execute(MethodInfo method, object[] arguments)
        {
            var interfaceMethod = GetInterfaceMethod(method);

            var invocation = new Invocation(backingObject, interceptors, interfaceMethod, arguments);
            invocation.Continue();

            return invocation.ReturnValue;
        }

        private MethodInfo GetInterfaceMethod(MethodInfo method)
        {
            interfaceMap = method.DeclaringType.GetInterfaceMap(typeof (T));

            int i = 0;
            foreach (var targetMethod in interfaceMap.TargetMethods)
            {
                if (targetMethod == method)
                    return interfaceMap.InterfaceMethods[i];
                i++;
            }
            throw new TypeLoadException("Could Not Find Method " + method.Name);
        }

        public void SetBackingObjectAndInterceptors(T backingObject, IInterceptor[] interceptors)
        {
            this.backingObject = backingObject;

            var list = interceptors.ToList();
            list.Reverse();
            this.interceptors = list.ToArray();
        }
    }
}