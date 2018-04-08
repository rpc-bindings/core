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
using DSerfozo.RpcBindings.Contract.Analyze;
using DSerfozo.RpcBindings.Contract.Execution.Model;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;
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
            var idGenerator = Mock.Of<IIdGenerator>();
            Mock.Get(idGenerator).Setup(_ => _.GetNextId()).Returns(1);

            var executor = new CallbackExecutor<object>(idGenerator, () => true, Mock.Of<IObservable<CallbackResult<object>>>());

            CallbackExecution<object> exec = null;
            ((IObservable<CallbackExecution<object>>)executor).Subscribe(
                callbackExecution => exec = callbackExecution);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            executor.Execute(new CallbackExecutionParameters<object>()
            {
                Binder = context => {
                    if (context.Direction == ObjectBindingDirection.In)
                    context.ObjectValue = context.NativeValue;
                    else
                    context.NativeValue = context.ObjectValue;
                },
                Id = 2,
                Parameters = new CallbackParameter[] { },
                ResultTargetType = null
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Assert.Equal(1, exec.ExecutionId);
            Assert.Equal(2, exec.FunctionId);
        }

        [Fact]
        public void ParametersBound()
        {
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => true, Mock.Of<IObservable<CallbackResult<object>>>());

            CallbackExecution<object> exec = null;
            ((IObservable<CallbackExecution<object>>)executor).Subscribe(
                callbackExecution => exec = callbackExecution);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            void Binder(BindingContext<object> context)
            {
                if (context.ObjectValue.GetType() == typeof(object))
                {
                    context.NativeValue = "str";
                }
            }

            executor.Execute(new CallbackExecutionParameters<object>()
            {
                Binder = Binder,
                Id = 2,
                Parameters = new[] { new CallbackParameter() {Value = new object()}, new CallbackParameter() { Value = new object() } },
                ResultTargetType = null
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Assert.Collection(exec.Parameters, s => Assert.Equal("str", (string)s), s => Assert.Equal("str", (string)s));
        }

        [Fact]
        public void ParametersBoundAndAnalyzed()
        {
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => true, Mock.Of<IObservable<CallbackResult<object>>>());

            CallbackExecution<object> exec = null;
            ((IObservable<CallbackExecution<object>>)executor).Subscribe(
                callbackExecution => exec = callbackExecution);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            void Binder(BindingContext<object> context)
            {
                if (context.BindValue != null)
                {
                    context.NativeValue = "str";
                }
                else
                {
                    context.NativeValue = "str2";
                }
            }

            executor.Execute(new CallbackExecutionParameters<object>()
            {
                Binder = Binder,
                Id = 2,
                Parameters = new[] { new CallbackParameter { Value = new object(), Bindable = new BindValueAttribute()}, new CallbackParameter { Value = new object() } },
                ResultTargetType = null
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Assert.Collection(exec.Parameters, s => Assert.Equal("str", (string)s), s => Assert.Equal("str2", (string)s));
        }

        [Fact]
        public async Task ResponseHandled()
        {
            var responses = new Subject<CallbackResult<object>>();
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => true, responses);

            void Binder(BindingContext<object> context)
            {
                if (context.TargetType == typeof(string))
                {
                    context.ObjectValue = "str";
                }
            }

            var task = executor.Execute(new CallbackExecutionParameters<object>()
            {
                Binder = Binder,
                Id = 2,
                Parameters = new CallbackParameter[] { },
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
        public async Task ErrorHandled()
        {
            var responses = new Subject<CallbackResult<object>>();
            var executor = new CallbackExecutor<object>(Mock.Of<IIdGenerator>(), () => true, responses);

            var task = executor.Execute(new CallbackExecutionParameters<object>
            {
                Binder = context => { },
                Id = 2,
                Parameters = new CallbackParameter[] { }
            });


            responses.OnNext(new CallbackResult<object>
            {
                ExecutionId = 0,
                Success = false,
                Error = "Error"
            });

            await Assert.ThrowsAsync<Exception>(() => task).ConfigureAwait(false);
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
            var idGenerator = Mock.Of<IIdGenerator>();
            Mock.Get(idGenerator).Setup(_ => _.GetNextId()).Returns(1);

            var executor = new CallbackExecutor<object>(idGenerator, () => false, Mock.Of<IObservable<CallbackResult<object>>>());

            CallbackExecution<object> exec = null;
            ((IObservable<CallbackExecution<object>>)executor).Subscribe(
                callbackExecution => exec = callbackExecution);

            await Assert.ThrowsAsync<InvalidOperationException>(() => executor.Execute(new CallbackExecutionParameters<object>
            {
                Binder = context => { },
                Id = 2,
                Parameters = new CallbackParameter[] { },
                ResultTargetType = null
            }));
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
