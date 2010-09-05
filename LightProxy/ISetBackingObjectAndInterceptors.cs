using System;

namespace LightProxy
{
    public interface ISetBackingObjectAndInterceptors<T>
    {
        void SetBackingObjectAndInterceptors(T backingObject, IInterceptor[] interceptors);
    }

    public class BlahBar
    {
        public object backingObject = new object();

        public void blah()
        {   
            backingObject.GetType();
        }
    }
}