using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class CallbackFactoryTests
    {
        [Fact]
        public void CallbackCreated()
        {
            var callbackFactory = new CallbackFactory()
            {
                CallbackExecutor = Mock.Of<ICallbackExecutor>()
            };

            Assert.NotNull(callbackFactory.Create(1));
        }
    }
}
