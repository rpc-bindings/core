using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract.Model;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class CallbackTests
    {
        [Fact]
        public void CallbackDisposed()
        {
            var executor = Mock.Of<ICallbackExecutor<object>>();
            var executorMock = Mock.Get(executor);

            new Callback<object>(1, executor, Mock.Of<IParameterBinder<object>>()).Dispose();

            executorMock.Verify(_ => _.DeleteCallback(1));
        }

        [Fact]
        public void CanExecuteFalseAfterDispose()
        {
            var executor = Mock.Of<ICallbackExecutor<object>>();
            var executorMock = Mock.Get(executor);

            var callback = new Callback<object>(1, executor, Mock.Of<IParameterBinder<object>>());
            callback.Dispose();

            Assert.False(callback.CanExecute);
        }

        [Fact]
        public async Task CallbackExecuted()
        {
            var executor = Mock.Of<ICallbackExecutor<object>>();
            var executorMock = Mock.Get(executor);
            executorMock.Setup(_ => _.Execute(It.Is<CallbackExecutionParameters<object>>(o => o.Parameters.Length == 1 && o.Binder is NoopObjectParameterBinder))).Returns(Task.FromResult(new object()));

            var callback = new Callback<object>(1, executor, new NoopObjectParameterBinder());

            var result = await callback.Execute(new object[] { "str" });

            Assert.NotNull(result);
        }
    }
}
