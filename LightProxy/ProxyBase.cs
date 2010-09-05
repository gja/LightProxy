namespace LightProxy
{
    public class ProxyBase<T>
    {
        private T backingObject;
        public T BackingObject
        {
            set { backingObject = value; }
        }

        private IInterceptor[] interceptors;
        public IInterceptor[] Interceptors
        {
            set { interceptors = value; }
        }
    }
}