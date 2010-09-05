using System;

namespace LightProxy
{
    public interface ISetBackingObjectAndInterceptors<T>
    {
        void SetBackingObjectAndInterceptors(T backingObject, IInterceptor[] interceptors);
    }   
}