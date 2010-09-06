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
            this.method = method;
            this.arguments = arguments;
            current = -1;
            Continue();
            return returnValue;
        }

        private MethodInfo method;
        public MethodInfo Method
        {
            get { return method; }
        }

        private object[] arguments;
        public object[] Arguments
        {
            get { return arguments; }
        }

        private object returnValue;
        public object ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }

        public void Continue()
        {
            if(current == last)
            {
                returnValue = method.Invoke(backingObject, arguments);
                return;
            }

            interceptors[++current].Intercept(this);
        }
    }
}