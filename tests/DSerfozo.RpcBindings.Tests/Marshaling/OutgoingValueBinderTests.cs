using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Analyze;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;
using DSerfozo.RpcBindings.Marshaling;
using Moq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Marshaling
{
    public class OutgoingValueBinderTests
    {
        [Fact]
        public void NextCalled()
        {
            var bindingRepo = Mock.Of<IBindingRepository>();

            var called = false;
            var bindingContext = new BindingContext<object>(ObjectBindingDirection.In, context => { });
            var binder = new OutgoingValueBinder<object>(context => { called = ReferenceEquals(context, bindingContext); }, bindingRepo);

            binder.Bind(bindingContext);

            Assert.True(called);
        }

        [Fact]
        public void OutgoingValueWithValueBindAnalyzed()
        {
            var bindingRepo = Mock.Of<IBindingRepository>();
            var bindingRepoMock = Mock.Get(bindingRepo);

            var objectValue = new object();
            var bindingContext = new BindingContext<object>(ObjectBindingDirection.Out, context => { })
            {
                BindValue = new BindValueAttribute(),
                ObjectValue = objectValue
            };
            var binder = new OutgoingValueBinder<object>(context => { }, bindingRepo);

            binder.Bind(bindingContext);

            bindingRepoMock.Verify(_ => _.AddBinding(objectValue, It.Is<AnalyzeOptions>(__ => __.AnalyzeProperties == false)), Times.Once);
        }

        [Fact]
        public void OutgoingValueWithoutValueBindNotAnalyzed()
        {
            var bindingRepo = Mock.Of<IBindingRepository>();
            var bindingRepoMock = Mock.Get(bindingRepo);

            var bindingContext = new BindingContext<object>(ObjectBindingDirection.Out, context => { });
            var binder = new OutgoingValueBinder<object>(context => { }, bindingRepo);

            binder.Bind(bindingContext);

            bindingRepoMock.Verify(_ => _.AddBinding(It.IsAny<object>(), It.IsAny<AnalyzeOptions>()), Times.Never);
        }

        [Fact]
        public void NullObjectValueSkipped()
        {
            var bindingRepo = Mock.Of<IBindingRepository>();
            var bindingRepoMock = Mock.Get(bindingRepo);

            var called = false;
            var bindingContext = new BindingContext<object>(ObjectBindingDirection.Out, context => { })
            {
                BindValue = new BindValueAttribute(),
                ObjectValue = null
            };
            var binder = new OutgoingValueBinder<object>(context => { called = ReferenceEquals(context, bindingContext); }, bindingRepo);

            binder.Bind(bindingContext);

            bindingRepoMock.Verify(_ => _.AddBinding(It.IsAny<object>(), It.IsAny<AnalyzeOptions>()), Times.Never);
        }

        [Fact]
        public void PropertyValuesExtractedIfSet()
        {
            var bindingRepo = Mock.Of<IBindingRepository>();
            var bindingRepoMock = Mock.Get(bindingRepo);

            var objectValue = new object();
            var bindingContext = new BindingContext<object>(ObjectBindingDirection.Out, context => { })
            {
                BindValue = new BindValueAttribute
                {
                    ExtractPropertyValues = true
                },
                ObjectValue = objectValue
            };
            var binder = new OutgoingValueBinder<object>(context => { }, bindingRepo);

            binder.Bind(bindingContext);

            bindingRepoMock.Verify(_ => _.AddBinding(objectValue, It.Is<AnalyzeOptions>(__ => __.AnalyzeProperties && __.ExtractPropertyValues)), Times.Once);
        }
    }
}
