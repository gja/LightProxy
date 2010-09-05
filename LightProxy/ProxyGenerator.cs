using System;

namespace LightProxy
{
    public class ProxyGenerator
    {
        public T GenerateProxy<T>(T backingObject, params IInterceptor[] interceptos) where T:class 
        {
            return null;
        }
    }

    public interface IInterceptor
    {
    }
}
