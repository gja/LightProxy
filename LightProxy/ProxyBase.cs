namespace LightProxy
{
    public class ProxyBase<T>
    {
        public T backingObject;
        public IInterceptor[] interceptors;
    }
}