using System;
using DSerfozo.RpcBindings.Contract;
using Moq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests
{
    public class BindingRepositoryTests
    {
        private class TestClass : IDisposable
        {
            public bool Disposed;

            public void Dispose()
            {
                Disposed = true;
            }
        }

        [Fact]
        public void BoundObjectsDisposed()
        {
            var testClass = new TestClass();
            using (var br = new BindingRepository(Mock.Of<IIdGenerator>()))
            {
                br.AddBinding("test", testClass);
            }

            Assert.True(testClass.Disposed);
        }

        [Fact]
        public void ThrowsIfDisposed()
        {
            var testClass = new TestClass();
            var br = new BindingRepository(Mock.Of<IIdGenerator>());
            br.Dispose();
            Assert.Throws<ObjectDisposedException>(() => br.AddBinding("test", testClass));
        }
    }
}
