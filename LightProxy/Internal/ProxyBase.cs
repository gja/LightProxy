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

        public object Execute(int i, object[] arguments/*, Func<object[], object> @delegate*/)
        {
            var type = GetType();
            var method = type.GetMethod("__LightProxy_Continue_" + methods[i].Name + i, new[]{typeof(object[])});
            return method.Invoke(this, new object[]{arguments});
//            return invocation.Start(methods[i], arguments);
        }

        public void InitializeProxy(T backingObject, IInterceptor[] interceptors, MethodInfo[] methods)
        {
            this.backingObject = backingObject;
            this.interceptors = interceptors;
            this.methods = methods;
            invocation = new Invocation(backingObject, interceptors);
        }        
    }

    public class FooBar
    {
        object backingObject = new object();

        void blah(Func<object[], object> action)
        {
            
        }

        object argv(object[] args)
        {
            foobar();
            return null;
        }

        void foobar()
        {
            blah(argv);
        }
    }
}