using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightProxy.Internal
{
    public class ProxyBase<T>
    {
        public T backingObject;
        public IInterceptor[] interceptors;
        protected Dictionary<MethodInfo, MethodInfo> methodMap = new Dictionary<MethodInfo, MethodInfo>();

        public object Execute(MethodInfo method, object[] arguments)
        {
            var invocation = new Invocation(backingObject, interceptors, methodMap[method], arguments);

            invocation.Continue();

            return invocation.ReturnValue;
        }

        public void InitializeProxy(T backingObject, IInterceptor[] interceptors)
        {
            this.backingObject = backingObject;

            var list = interceptors.ToList();
            list.Reverse();
            this.interceptors = list.ToArray();

            var interfaceMap = GetType().GetInterfaceMap(typeof (T));
            for (int i = 0; i < interfaceMap.InterfaceMethods.Count(); i++)
                methodMap[interfaceMap.TargetMethods[i]] = interfaceMap.InterfaceMethods[i];
        }        
    }
}