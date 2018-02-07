using Moq;
using System;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Extensions;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Extensions
{
    public class BindingRepositoryExtensionsTests
    {
        [Fact]
        public void BindingRepositoryReadOnly()
        {
            var repository = Mock.Of<IBindingRepository>();
            var readOnly = repository.AsReadOnly();

            Assert.Throws<InvalidOperationException>(() => readOnly.AddBinding("key", new object()));
        }

        [Fact]
        public void NotDisposable()
        {
            var repository = Mock.Of<IBindingRepository>();
            var readOnly = repository.AsReadOnly();

            Assert.Throws<InvalidOperationException>(() => readOnly.Dispose());
        }
    }
}
