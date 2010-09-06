using System.Collections.Generic;
using System.Reflection;

namespace LightProxy.Internal
{
    internal class Invocation : IInvocation
    {
        private readonly object backingObject;
        private readonly Queue<IInterceptor> remainingInterceptors;

        public Invocation(object backingObject, IEnumerable<IInterceptor> interceptors, MethodInfo method, object[] arguments)
        {
            this.backingObject = backingObject;
            remainingInterceptors = new Queue<IInterceptor>(interceptors);
            Method = method;
            Arguments = arguments;
        }

        public MethodInfo Method { get; private set; }
        public object[] Arguments { get; private set; }
        public object ReturnValue { get; set; }
        
        public void Continue()
        {
            if(remainingInterceptors.Count == 0)
            {
                ReturnValue = Method.Invoke(backingObject, Arguments);
                return;
            }

            remainingInterceptors.Dequeue().Intercept(this);
        }
    }
}