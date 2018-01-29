using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Model;
using Moq;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract.Model;
using DSerfozo.RpcBindings.Execution;
using DSerfozo.RpcBindings.Execution.Model;
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
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), Mock.Of<IObservable<CallbackResult<object>>>());

            DeleteCallback delete = null;
            ((IObservable<DeleteCallback>)executor).Subscribe(
                deleteCallback => delete = deleteCallback);

            executor.DeleteCallback(1);

            Assert.Equal(1, delete.FunctionId);
        }

        [Fact]
        public void ExecuteSent()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);
            var idGenerator = Mock.Of<IIdGenerator>();
            Mock.Get(idGenerator).Setup(_ => _.GetNextId()).Returns(1);

            var executor = new CallbackExecutor<object>(idGenerator, Mock.Of<IObservable<CallbackResult<object>>>());

            CallbackExecution<object> exec = null;
            ((IObservable<CallbackExecution<object>>) executor).Subscribe(
                callbackExecution => exec = callbackExecution);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            executor.Execute(new CallbackExecutionParameters<object>()
            {
                Binder = new NoopObjectParameterBinder(),
                Id = 2,
                Parameters = new object[] { },
                ResultTargetType = null
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Assert.Equal(1, exec.ExecutionId);
            Assert.Equal(2, exec.FunctionId);
        }

        [Fact]
        public void ParametersBound()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);
            var parameterBinder = Mock.Of<IParameterBinder<object>>();
            var parameterBinderMock = Mock.Get(parameterBinder);

            parameterBinderMock.Setup(_ => _.BindToWire(It.IsAny<object>())).Returns("str");

            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), Mock.Of<IObservable<CallbackResult<object>>>());

            CallbackExecution<object> exec = null;
            ((IObservable<CallbackExecution<object>>)executor).Subscribe(
                callbackExecution => exec = callbackExecution);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            executor.Execute(new CallbackExecutionParameters<object>()
            {
                Binder = parameterBinder,
                Id = 2,
                Parameters = new object[] { new object(), new object() },
                ResultTargetType = null
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Assert.Collection(exec.Parameters, s => Assert.Equal("str", (string)s), s => Assert.Equal("str", (string)s));
        }

        [Fact]
        public async Task ResponseHandled()
        {
            var parameterBinder = Mock.Of<IParameterBinder<object>>();
            var parameterBinderMock = Mock.Get(parameterBinder);
            parameterBinderMock.Setup(_ => _.BindToNet(It.Is<ParameterBinding<object>>(s => s.TargetType == typeof(string)))).Returns("str");

            var responses = new Subject<CallbackResult<object>>();
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), responses);

            var task =  executor.Execute(new CallbackExecutionParameters<object>()
            {
                Binder = parameterBinder,
                Id = 2,
                Parameters = new object[] { },
                ResultTargetType = typeof(string)
            });

            responses.OnNext(new CallbackResult<object>
            {

                ExecutionId = 0,
                Success = true,
                Result = "str"
            });

            Assert.Equal("str", await task);
        }

        [Fact]
        public void ErrorHandled()
        {
            var connection = Mock.Of<IConnection<object>>();
            var connectionMock = Mock.Get(connection);

            var responses = new Subject<CallbackResult<object>>();
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), responses);

            var task = executor.Execute(new CallbackExecutionParameters<object>()
            {
                Binder = new NoopObjectParameterBinder(),
                Id = 2,
                Parameters = new object[] { }
            });

            responses.OnNext(new CallbackResult<object>
            {
                ExecutionId = 2,
                Success = false,
                Error = "Error"
            });

            Assert.ThrowsAsync<Exception>(() => task);
        }
    }
}
