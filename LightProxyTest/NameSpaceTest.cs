using NUnit.Framework;

namespace first
{
    public interface IFoo
    {
        
    }
}

namespace second
{
    public interface IFoo
    {
        
    }
}

namespace LightProxyTest
{   
    [TestFixture]
    public class NameSpaceTest : ProxyGeneratorTestBase
    {
        [Test]
        public void ShouldBeAbleToOverrideTwoInterfacesWithSameName()
        {
            generator.GenerateProxy<first.IFoo>(null);
            generator.GenerateProxy<second.IFoo>(null);
        }
    }
}
