namespace LightProxy
{
    public class ProxyBase<T>
    {
        protected readonly T backingObject;
        protected readonly IInterceptor[] interceptors;

        public ProxyBase(T backingObject, IInterceptor[] interceptors)
        {
            this.backingObject = backingObject;
            this.interceptors = interceptors;
        }
    }
}