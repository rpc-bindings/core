using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class CallbackTests
    {
        [Fact]
        public void CallbackDisposed()
        {
            var executor = Mock.Of<ICallbackExecutor>();
            var executorMock = Mock.Get(executor);

            new Callback(1, executor).Dispose();

            executorMock.Verify(_ => _.DeleteCallback(1));
        }

        [Fact]
        public void CanExecuteFalseAfterDispose()
        {
            var executor = Mock.Of<ICallbackExecutor>();
            var executorMock = Mock.Get(executor);

            var callback = new Callback(1, executor);
            callback.Dispose();

            Assert.False(callback.CanExecute);
        }

        [Fact]
        public async Task CallbackExecuted()
        {
            var executor = Mock.Of<ICallbackExecutor>();
            var executorMock = Mock.Get(executor);
            executorMock.Setup(_ => _.Execute(1, It.Is<object[]>(o => o.Length == 1))).Returns(Task.FromResult(new object()));

            var callback = new Callback(1, executor);

            var result = await callback.Execute(new object[] { "str" });

            Assert.NotNull(result);
        }
    }
}
