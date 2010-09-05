namespace LightProxy
{
    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);
    }
}