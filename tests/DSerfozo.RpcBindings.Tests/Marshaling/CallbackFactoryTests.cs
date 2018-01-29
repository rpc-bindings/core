using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using System;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract.Model;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class CallbackFactoryTests
    {
        [Fact]
        public void CallbackCreated()
        {
            var callbackFactory = new CallbackFactory<object>(Mock.Of<ICallbackExecutor<object>>());

            Assert.NotNull(callbackFactory.CreateCallback(1, null, new NoopObjectParameterBinder()));
        }

        [Fact]
        public async Task DelegateCreated()
        {
            var callbackExecutor = Mock.Of<ICallbackExecutor<object>>();
            var callbackExecutorMock = Mock.Get(callbackExecutor);
            callbackExecutorMock.Setup(_ => _.Execute(It.Is<CallbackExecutionParameters<object>>(c => c.Id == 9 && (string)c.Parameters[0] == "input"))).Returns(Task.FromResult<object>("result"));
            var callbackFactory = new CallbackFactory<object>(callbackExecutor);

            Func<string, Task<string>> del = (Func<string, Task<string>>) callbackFactory.CreateCallback(9,
                typeof(Func<string, Task<string>>), Mock.Of<IParameterBinder<object>>());
            Assert.Equal("result", await del("input"));
        }
    }
}
