using System;
using System.Reflection;

namespace LightProxy
{
    public class ProxyBase<T>
    {
        public T backingObject;
        public IInterceptor[] interceptors;

        public object Execute(MethodInfo method, object[] arguments)
        {
            var interfaceMethod = GetInterfaceMethod(method);
            
            return interfaceMethod.Invoke(backingObject, arguments);
        }

        private MethodInfo GetInterfaceMethod(MethodInfo method)
        {
            var map = method.DeclaringType.GetInterfaceMap(typeof (T));

            int i = 0;
            foreach (var targetMethod in map.TargetMethods)
            {
                if (targetMethod == method)
                    return map.InterfaceMethods[i];
                i++;
            }
            throw new TypeLoadException("Could Not Find Method " + method.Name);
        }
    }
}