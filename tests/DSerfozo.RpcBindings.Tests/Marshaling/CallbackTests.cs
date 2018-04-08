using System;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Execution.Model;
using DSerfozo.RpcBindings.Contract.Marshaling;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class CallbackTests
    {
        private class TestCallback : CallbackBase<object>
        {
            public TestCallback(long id, ICallbackExecutor<object> executor, BindingDelegate<object> parameterBinder, Type resultType) : base(id, executor, parameterBinder, resultType)
            {
            }
        }

        [Fact]
        public void CallbackDisposed()
        {
            var executor = Mock.Of<ICallbackExecutor<object>>();
            var executorMock = Mock.Get(executor);
            executorMock.SetupGet(_ => _.CanExecute).Returns(true);

            new Callback<object>(1, executor, context => { }).Dispose();

            executorMock.Verify(_ => _.DeleteCallback(1));
        }

        [Fact]
        public void DisposeDoesNotDeleteIfCanExecuteFalse()
        {
            var executor = Mock.Of<ICallbackExecutor<object>>();
            var executorMock = Mock.Get(executor);
            executorMock.SetupGet(_ => _.CanExecute).Returns(false);

            new Callback<object>(1, executor, context => { }).Dispose();

            executorMock.Verify(_ => _.DeleteCallback(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CanExecuteFalseAfterDispose()
        {
            var executor = Mock.Of<ICallbackExecutor<object>>();
            var executorMock = Mock.Get(executor);

            var callback = new Callback<object>(1, executor, context => { });
            callback.Dispose();

            Assert.False(callback.CanExecute);
        }

        [Fact]
        public void CanExecuteFalseWhenExecutorReturnsFalse()
        {
            var executor = Mock.Of<ICallbackExecutor<object>>();
            var executorMock = Mock.Get(executor);
            executorMock.SetupGet(_ => _.CanExecute).Returns(false);

            var callback = new Callback<object>(1, executor, context => { });

            Assert.False(callback.CanExecute);
        }

        [Fact]
        public async Task CallbackExecuted()
        {
            var executor = Mock.Of<ICallbackExecutor<object>>();
            var executorMock = Mock.Get(executor);
            BindingDelegate<object> parameterBinder = context => { };
            executorMock
                .Setup(_ => _.Execute(It.Is<CallbackExecutionParameters<object>>(o =>
                    o.Parameters.Length == 1 && o.Binder == parameterBinder && o.ResultTargetType == typeof(string)))).Returns(Task.FromResult(new object()));
            executorMock.SetupGet(_ => _.CanExecute).Returns(true);

            var callback = new TestCallback(1, executor, parameterBinder, typeof(string));

            var result = await callback.ExecuteAsync("str");

            Assert.NotNull(result);
        }
    }
}
