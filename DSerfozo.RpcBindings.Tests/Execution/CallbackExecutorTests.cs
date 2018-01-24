using DSerfozo.RpcBindings.Calling;
using DSerfozo.RpcBindings.Communication;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Model;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Execution
{
    public class CallbackExecutorTests
    {
        [Fact]
        public void CallbackDeleted()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);
            var executor = new CallbackExecutor<object>(connection, new NoopObjectParameterBinder());

            executor.DeleteCallback(1);

            connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(req => req.DeleteCallback != null && req.DeleteCallback.Id == 1)));
        }

        [Fact]
        public void ExecuteSent()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            var executor = new CallbackExecutor<object>(connection, new NoopObjectParameterBinder());

            executor.Execute(1, new object[] { "str" });

            connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(req => req.CallbackExecution != null && req.CallbackExecution.Id == 1 && req.CallbackExecution.Parameters.Length == 1)));
        }

        [Fact]
        public void ParametersBound()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);
            var parameterBinder = Mock.Of<IParameterBinder<object>>();
            var parameterBinderMock = Mock.Get(parameterBinder);

            parameterBinderMock.Setup(_ => _.BindToNet(It.IsAny<ParameterBinding<object>>())).Returns("str");

            var executor = new CallbackExecutor<object>(connection, parameterBinder);

            executor.Execute(1, new object[] { new object(), new object() });

            connectionMock.Verify(_ => _.Send(It.Is<RpcRequest<object>>(req => req.CallbackExecution != null &&
                req.CallbackExecution.Id == 1 &&
                req.CallbackExecution.Parameters.Length == 2 &&
                req.CallbackExecution.Parameters.All(p => !(p is string)))));
        }

        [Fact]
        public async Task ResponseHandled()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            var executor = new CallbackExecutor<object>(connection, new NoopObjectParameterBinder());

            var task = executor.Execute(1, new object[] { "str" });

            connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
            {
                CallbackResult = new CallbackResult<object>
                {
                    Id = 1,
                    IsSuccess = true,
                    Result = "str"
                }
            });

            Assert.Equal("str", await task);
        }

        [Fact]
        public void ErrorHandled()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            var executor = new CallbackExecutor<object>(connection, new NoopObjectParameterBinder());

            var task = executor.Execute(1, new object[] { "str" });

            connectionMock.Raise(_ => _.RpcResponse += null, new RpcResponse<object>
            {
                CallbackResult = new CallbackResult<object>
                {
                    Id = 1,
                    IsSuccess = false,
                    Error = "Error"
                }
            });

            Assert.ThrowsAsync<Exception>(() => task);
        }
    }
}
