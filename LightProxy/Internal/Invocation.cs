using System.Collections.Generic;
using System.Reflection;

namespace LightProxy.Internal
{
    internal class Invocation : IInvocation
    {
        private readonly object backingObject;
        private readonly IInterceptor[] interceptors;
        private readonly int last;
        private int current;

        public Invocation(object backingObject, IInterceptor[] interceptors)
        {
            this.backingObject = backingObject;
            this.interceptors = interceptors;
            last = interceptors.Length - 1;            
        }

        public object Start(MethodInfo method, object[] arguments)
        {
            Method = method;
            Arguments = arguments;
            current = -1;
            Continue();
            return ReturnValue;
        }

        public MethodInfo Method { get; private set; }
        public object[] Arguments { get; private set; }
        public object ReturnValue { get; set; }
        
        public void Continue()
        {
            if(current == last)
            {
                ReturnValue = Method.Invoke(backingObject, Arguments);
                return;
            }

            interceptors[++current].Intercept(this);
        }
    }
}