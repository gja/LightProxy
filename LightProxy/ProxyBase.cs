using System;
using System.Reflection;

namespace LightProxy
{
    public class ProxyBase<T>
    {
        public T backingObject;
        public IInterceptor[] interceptors;

        public int Execute(MethodInfo method, T backingObject, IInterceptor[] interceptors, object[] arguments)
        {
            return 42;
        }
    }

    public class Tmp : ProxyBase<object>
    {
        public int blah(object a, object b, object c, object d, object e)
        {
            throw new Exception();
        }
    }
}