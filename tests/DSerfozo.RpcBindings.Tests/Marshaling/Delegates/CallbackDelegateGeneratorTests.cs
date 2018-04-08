using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Execution.Model;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Marshaling.Delegates;
using Moq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling.Delegates
{
    public class CallbackDelegateGeneratorTests
    {
        public delegate Task<string> TestDelegate(string param);

        public delegate Task TestBindingDelegate([BindValue]object obj);

        [Fact]
        public void ReturnTypeSet()
        {
            var gen = new CallbackDelegateGenerator<object>();
            var callbackExecutorMock = new Mock<ICallbackExecutor<object>>();
            callbackExecutorMock.SetupGet(_ => _.CanExecute).Returns(true);
            var generated = (TestDelegate)gen.Generate(typeof(TestDelegate), 1, callbackExecutorMock.Object, context => { });

            generated("");

            callbackExecutorMock.Verify(_ => _.Execute(It.Is<CallbackExecutionParameters<object>>(__ => __.ResultTargetType == typeof(string))));
        }

        [Fact]
        public void BindValuesSet()
        {
            var gen = new CallbackDelegateGenerator<object>();
            var callbackExecutorMock = new Mock<ICallbackExecutor<object>>();
            callbackExecutorMock.SetupGet(_ => _.CanExecute).Returns(true);
            
            var generated = (TestBindingDelegate)gen.Generate(typeof(TestBindingDelegate), 1, callbackExecutorMock.Object, context => { });

            generated(new object());

            callbackExecutorMock.Verify(_ =>
                _.Execute(It.Is<CallbackExecutionParameters<object>>(__ => __.Parameters.Single().Bindable != null)));
        }

        [Fact]
        public void FailsForNonTaskReturnType()
        {
            var gen = new CallbackDelegateGenerator<object>();
            var callbackExecutorMock = new Mock<ICallbackExecutor<object>>();
            callbackExecutorMock.SetupGet(_ => _.CanExecute).Returns(true);

            Assert.Throws<InvalidOperationException>(() => gen.Generate(typeof(Action), 1, callbackExecutorMock.Object,
                context => { }));
        }
    }
}
