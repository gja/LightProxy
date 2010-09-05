using System.Reflection;

namespace LightProxy
{
    public interface IInvocation
    {
        MethodInfo Method { get; }
        object[] Arguments { get; }
        object ReturnValue { get; set; }

        void Continue();
    }
}