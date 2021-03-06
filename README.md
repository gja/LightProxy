LightProxy
==========

This is a lightweight library for generating proxy objects. It aims to be faster than DynamicProxy, with a subset of the features. It only has one mode, proxy an interface with a concrete backing object.

Examples
--------

    // Hold onto this object. We emit one assembly per generator.
    // This generator can be re-used
    var generator = new ProxyGenerator();
    
    IFoo foo = generator.GenerateProxy<IFoo, Foo>(new Foo(), interceptor1, interceptor2);
    
    class Interceptor1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
        // Do something before
    	invocation.Continue();
    	// Do something after
        }
    }
    
Website
-------
http://github.com/gja/LightProxy

License
-------
    Copyright (c) 2010 Tejas Dinkar
    
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at
    
    http://www.apache.org/licenses/LICENSE-2.0
    
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
