using System.Text;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;
using DSerfozo.RpcBindings.Marshaling;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class ObjectBinderBuilderTests
    {
        [Fact]
        public void DefaultComponentExists()
        {
            var builder = new ObjectBinderBuilder<object>();
            var binder = builder.Build();

            Assert.NotNull(binder);
        }

        [Fact]
        public void ComponentsCalledInOrder()
        {
            var stringBuilder = new StringBuilder();
            var builder = new ObjectBinderBuilder<object>();
            builder.Use(next => ctx =>
            {
                stringBuilder.Append("1");
                next(ctx);
            });

            builder.Use(next => ctx =>
            {
                stringBuilder.Append("2");
                next(ctx);
            });
            var binder = builder.Build();

            binder(new BindingContext<object>(ObjectBindingDirection.In, null));

            Assert.Equal("12", stringBuilder.ToString());
        }

    }
}
