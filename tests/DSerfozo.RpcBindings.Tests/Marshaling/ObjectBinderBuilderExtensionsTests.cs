using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;
using DSerfozo.RpcBindings.Extensions;
using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Model;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class ObjectBinderBuilderExtensionsTests
    {
        private class MissingBindMethod
        {
            
        }

        private class WrongBindMethodParameters
        {
            public void Bind()
            {

            }
        }

        private class WrongBindMethodGenericParameter
        {
            public void Bind(BindingContext<string> ctx)
            {

            }
        }

        private class Binder
        {
            private readonly BindingDelegate<object> next;

            public Binder(BindingDelegate<object> next)
            {
                this.next = next;
            }

            public void Bind(BindingContext<object> ctx)
            {
                ctx.ObjectValue = "string";

                next(ctx);
            }
        }

        private class BinderWithCtrArgs
        {
            private readonly BindingDelegate<object> next;
            private readonly string arg;

            public BinderWithCtrArgs(BindingDelegate<object> next, string arg)
            {
                this.next = next;
                this.arg = arg;
            }

            public void Bind(BindingContext<object> ctx)
            {
                ctx.ObjectValue = arg;

                next(ctx);
            }
        }

        [Fact]
        public void MissingBindMethodThrows()
        {
            var binder = new ObjectBinderBuilder<object>();
            binder.Use(typeof(MissingBindMethod));

            Assert.Throws<InvalidOperationException>(() => binder.Build());
        }

        [Fact]
        public void WrongBindMethodParameterThrows()
        {
            var binder = new ObjectBinderBuilder<object>();
            binder.Use(typeof(WrongBindMethodParameters));

            Assert.Throws<InvalidOperationException>(() => binder.Build());
        }

        [Fact]
        public void WrongBindMethodThrows()
        {
            var binder = new ObjectBinderBuilder<object>();
            binder.Use(typeof(WrongBindMethodGenericParameter));

            Assert.Throws<InvalidOperationException>(() => binder.Build());
        }

        [Fact]
        public void BindCalled()
        {
            var binder = new ObjectBinderBuilder<object>();
            binder.Use(typeof(Binder));

            var ctx = new BindingContext<object>(ObjectBindingDirection.In, null);

            binder.Build()(ctx);

            Assert.Equal("string", ctx.ObjectValue as string);
        }

        [Fact]
        public void NextAvailable()
        {
            var binder = new ObjectBinderBuilder<object>();
            binder.Use(typeof(Binder));
            var called = false;
            binder.Use(n => c => { called = true; });

            var ctx = new BindingContext<object>(ObjectBindingDirection.In, null);

            binder.Build()(ctx);

            Assert.True(called);
        }

        [Fact]
        public void ConstructorArgsPassed()
        {
            var binder = new ObjectBinderBuilder<object>();
            binder.Use(typeof(BinderWithCtrArgs), "str");

            var ctx = new BindingContext<object>(ObjectBindingDirection.In, null);

            binder.Build()(ctx);

            Assert.Equal("str", ctx.ObjectValue as string);
        }
    }
}
