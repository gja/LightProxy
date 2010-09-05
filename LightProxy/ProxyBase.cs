using System;

namespace LightProxy
{
    public class ProxyBase<T>
    {
        protected T backingObject;
        public T BackingObject
        {
            set { backingObject = value; }
        }

        protected IInterceptor[] interceptors;
        public IInterceptor[] Interceptors
        {
            set { interceptors = value; }
        }
    }

    public interface ISetBackingObjectAndInterceptors<T>
    {
        void SetBackingObjectAndInterceptors(T backingObject, IInterceptor[] interceptors);
    }
}