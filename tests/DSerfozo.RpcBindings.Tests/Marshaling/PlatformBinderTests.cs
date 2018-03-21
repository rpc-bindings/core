using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class PlatformBinderTests
    {
        [Fact]
        public void BindToNetCalled()
        {
            var platformBinderMock = new Mock<IPlatformBinder<object>>();
            platformBinderMock.Setup(_ =>
                _.BindToNet(It.Is<Binding<object>>(__ =>
                    __.TargetType == typeof(string) && (string)__.Value == "str"))).Returns("sun");
            var binding = new PlatformBinder<object>(context => { }, platformBinderMock.Object);

            var bindingContext = new BindingContext<object>(ObjectBindingDirection.In, null)
            {
                TargetType = typeof(string),
                NativeValue = "str"
            };
            binding.Bind(bindingContext);

            Assert.Equal("sun", bindingContext.ObjectValue);
        }

        [Fact]
        public void BindToWireCalled()
        {
            var platformBinderMock = new Mock<IPlatformBinder<object>>();
            platformBinderMock.Setup(_ =>
                _.BindToWire(It.Is<object>(__ =>(string)__ == "string"))).Returns("str");
            var binding = new PlatformBinder<object>(context => { }, platformBinderMock.Object);

            var bindingContext = new BindingContext<object>(ObjectBindingDirection.Out, null)
            {
                ObjectValue = "string"
            };
            binding.Bind(bindingContext);

            Assert.Equal("str", bindingContext.NativeValue);
        }
    }
}
