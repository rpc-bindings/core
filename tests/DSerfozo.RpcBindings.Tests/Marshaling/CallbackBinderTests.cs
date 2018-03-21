using System;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;
using DSerfozo.RpcBindings.Contract.Model;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class CallbackBinderTests
    {
        [Fact]
        public void OutDirectionCallsNext()
        {
            var called = false;
            var callbackFactoryMock = new Mock<ICallbackFactory<object>>();
            var binder = new CallbackBinder<object>(context => called = true, callbackFactoryMock.Object);

            var ctx = new BindingContext<object>(ObjectBindingDirection.Out, context => { });
            ctx.ObjectValue = "str";

            binder.Bind(ctx);

            Assert.True(called);
        }

        [Fact]
        public void NonCallbackTargetTypeCallsNext()
        {
            var called = false;
            var callbackFactoryMock = new Mock<ICallbackFactory<object>>();
            var binder = new CallbackBinder<object>(context => called = true, callbackFactoryMock.Object);

            var ctx = new BindingContext<object>(ObjectBindingDirection.In, context => { });
            ctx.TargetType = typeof(string);

            binder.Bind(ctx);

            Assert.True(called);
        }

        [Fact]
        public void NonTypedCallbackCallsFactory()
        {
            BindingDelegate<object> bindingDelegate = context =>
            {
                
            };
            var callbackFactoryMock = new Mock<ICallbackFactory<object>>();
            callbackFactoryMock.Setup(_ => _.CreateCallback(1, null, bindingDelegate)).Returns("str");
            var binder = new CallbackBinder<object>(context => {
                context.ObjectValue = new CallbackDescriptor
                {
                    FunctionId = 1
                };
            }, callbackFactoryMock.Object);

            var ctx = new BindingContext<object>(ObjectBindingDirection.In, bindingDelegate);
            ctx.TargetType = typeof(ICallback);

            binder.Bind(ctx);

            Assert.Equal("str", ctx.ObjectValue);
        }

        [Fact]
        public void TypedCallbackCallsFactory()
        {
            BindingDelegate<object> bindingDelegate = context =>
            {

            };
            var callbackFactoryMock = new Mock<ICallbackFactory<object>>();
            callbackFactoryMock.Setup(_ => _.CreateCallback(1, typeof(Action), bindingDelegate)).Returns("str");
            var binder = new CallbackBinder<object>(context => {
                context.ObjectValue = new CallbackDescriptor
                {
                    FunctionId = 1
                };
            }, callbackFactoryMock.Object);

            var ctx = new BindingContext<object>(ObjectBindingDirection.In, bindingDelegate);
            ctx.TargetType = typeof(Action);

            binder.Bind(ctx);

            Assert.Equal("str", ctx.ObjectValue);
        }
    }
}
