using System;
using System.Collections.Generic;
using System.Reflection;

namespace LightProxy.Internal
{
    internal class Invocation : IInvocation
    {
        private readonly IInterceptor[] interceptors;
        private readonly int last;
        private int current;
        private Func<object[], object> continueDelegate;

        public Invocation(object backingObject, IInterceptor[] interceptors)
        {
            this.interceptors = interceptors;
            last = interceptors.Length - 1;
        }

        public object Start(MethodInfo method, object[] arguments, Func<object[], object> continueDelegate)
        {
            this.method = method;
            this.arguments = arguments;
            this.continueDelegate = continueDelegate;
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
                returnValue = continueDelegate(arguments);
                return;
            }

            interceptors[++current].Intercept(this);
        }
    }
}