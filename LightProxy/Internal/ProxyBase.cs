using System;
using System.Linq;
using System.Reflection;

namespace LightProxy.Internal
{
    public class ProxyBase<T>
    {
        public T backingObject;
        public IInterceptor[] interceptors;
        private Invocation invocation;
        protected MethodInfo[] methods;

        public object Execute(int i, object[] arguments, Func<object[], object> continueDelegate)
        {
            //TODO: Creating a new invocation each time seems to be marginally slower
            return invocation.Start(methods[i], arguments, continueDelegate);
        }

        public void InitializeProxy(T backingObject, IInterceptor[] interceptors, MethodInfo[] methods)
        {
            this.backingObject = backingObject;
            this.interceptors = interceptors;
            this.methods = methods;
            invocation = new Invocation(interceptors);
        }        
    }
}