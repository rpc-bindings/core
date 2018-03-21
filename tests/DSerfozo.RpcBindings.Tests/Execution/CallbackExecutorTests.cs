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
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => true, Mock.Of<IObservable<CallbackResult<object>>>());

            DeleteCallback delete = null;
            ((IObservable<DeleteCallback>)executor).Subscribe(
                deleteCallback => delete = deleteCallback);

            executor.DeleteCallback(1);

            Assert.Equal(1, delete.FunctionId);
        }

        [Fact]
        public void ExecuteSent()
        {
//            var idGenerator = Mock.Of<IIdGenerator>();
//            Mock.Get(idGenerator).Setup(_ => _.GetNextId()).Returns(1);

//            var executor = new CallbackExecutor<object>(idGenerator, () => true, Mock.Of<IObservable<CallbackResult<object>>>());

//            CallbackExecution<object> exec = null;
//            ((IObservable<CallbackExecution<object>>) executor).Subscribe(
//                callbackExecution => exec = callbackExecution);

//#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//            executor.Execute(new CallbackExecutionParameters<object>()
//            {
//                Binder = new NoopObjectPlatformBinder(),
//                Id = 2,
//                Parameters = new object[] { },
//                ResultTargetType = null
//            });
//#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

//            Assert.Equal(1, exec.ExecutionId);
//            Assert.Equal(2, exec.FunctionId);
        }

        [Fact]
        public void ParametersBound()
        {
//            var parameterBinder = Mock.Of<IPlatformBinder<object>>();
//            var parameterBinderMock = Mock.Get(parameterBinder);

//            parameterBinderMock.Setup(_ => _.BindToWire(It.IsAny<object>())).Returns("str");

//            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => true, Mock.Of<IObservable<CallbackResult<object>>>());

//            CallbackExecution<object> exec = null;
//            ((IObservable<CallbackExecution<object>>)executor).Subscribe(
//                callbackExecution => exec = callbackExecution);

//#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//            executor.Execute(new CallbackExecutionParameters<object>()
//            {
//                Binder = parameterBinder,
//                Id = 2,
//                Parameters = new object[] { new object(), new object() },
//                ResultTargetType = null
//            });
//#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

//            Assert.Collection(exec.Parameters, s => Assert.Equal("str", (string)s), s => Assert.Equal("str", (string)s));
        }

        [Fact]
        public async Task ResponseHandled()
        {
            //var parameterBinder = Mock.Of<IPlatformBinder<object>>();
            //var parameterBinderMock = Mock.Get(parameterBinder);
            //parameterBinderMock.Setup(_ => _.BindToNet(It.Is<PlatformBinding<object>>(s => s.TargetType == typeof(string)))).Returns("str");

            //var responses = new Subject<CallbackResult<object>>();
            //var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => true, responses);

            //var task =  executor.Execute(new CallbackExecutionParameters<object>()
            //{
            //    Binder = parameterBinder,
            //    Id = 2,
            //    Parameters = new object[] { },
            //    ResultTargetType = typeof(string)
            //});

            //responses.OnNext(new CallbackResult<object>
            //{

            //    ExecutionId = 0,
            //    Success = true,
            //    Result = "str"
            //});

            //Assert.Equal("str", await task);
        }

        [Fact]
        public async Task ErrorHandled()
        {
            //var responses = new Subject<CallbackResult<object>>();
            //var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => true, responses);

            //var task = executor.Execute(new CallbackExecutionParameters<object>
            //{
            //    Binder = new NoopObjectPlatformBinder(),
            //    Id = 2,
            //    Parameters = new object[] { }
            //});


            //responses.OnNext(new CallbackResult<object>
            //{
            //    ExecutionId = 0,
            //    Success = false,
            //    Error = "Error"
            //});

            //await Assert.ThrowsAsync<Exception>(() => task).ConfigureAwait(false);
        }

        [Fact]
        public void CanExecuteReturned()
        {
            var responses = new Subject<CallbackResult<object>>();
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => false, responses);

            Assert.False(executor.CanExecute);
        }

        [Fact]
        public async Task ExecuteFails()
        {
            //var idGenerator = Mock.Of<IIdGenerator>();
            //Mock.Get(idGenerator).Setup(_ => _.GetNextId()).Returns(1);

            //var executor = new CallbackExecutor<object>(idGenerator, () => false, Mock.Of<IObservable<CallbackResult<object>>>());

            //CallbackExecution<object> exec = null;
            //((IObservable<CallbackExecution<object>>)executor).Subscribe(
            //    callbackExecution => exec = callbackExecution);

            //await Assert.ThrowsAsync<InvalidOperationException>(() => executor.Execute(new CallbackExecutionParameters<object>
            //{
            //    Binder = new NoopObjectPlatformBinder(),
            //    Id = 2,
            //    Parameters = new object[] { },
            //    ResultTargetType = null
            //}));
        }

        [Fact]
        public void DeletionFails()
        {
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => false, Mock.Of<IObservable<CallbackResult<object>>>());

            DeleteCallback delete = null;
            ((IObservable<DeleteCallback>)executor).Subscribe(
                deleteCallback => delete = deleteCallback);

            Assert.Throws<InvalidOperationException>(() => executor.DeleteCallback(1));
        }
    }
}
