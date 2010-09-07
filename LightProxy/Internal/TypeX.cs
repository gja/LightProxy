using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightProxy.Internal
{
    internal static class TypeX
    {
        internal static MethodInfo[] GetAllMethods(this Type type)
        {
            return new ClassIterator().FindAll(type).SelectMany(iface => iface.GetMethods()).ToArray();
        }
    }

    internal class ClassIterator
    {
        private List<Type> visitedTypes = new List<Type>();

        public IEnumerable<Type> FindAll(Type type)
        {
            if(visitedTypes.Contains(type))
                yield break;

            visitedTypes.Add(type);
            yield return type;

            foreach (var @interface in type.GetInterfaces())
                foreach (var next in FindAll(@interface))
                    yield return next;
        }
    }
}
