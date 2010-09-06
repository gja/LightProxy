using System.Reflection;

namespace LightProxy.Internal
{
    public class ProxyBase<T>
    {
        public T backingObject;
        public IInterceptor[] interceptors;
        private Invocation invocation;
        protected MethodInfo[] methods;

        public object Execute(int i, object[] arguments)
        {
            return invocation.Start(methods[i], arguments);
        }

        public void InitializeProxy(T backingObject, IInterceptor[] interceptors, MethodInfo[] methods)
        {
            this.backingObject = backingObject;
            this.interceptors = interceptors;
            this.methods = methods;
            invocation = new Invocation(backingObject, interceptors);
        }        
    }
}