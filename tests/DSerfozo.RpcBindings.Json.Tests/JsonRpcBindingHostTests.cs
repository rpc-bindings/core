using System;
using DSerfozo.RpcBindings.Contract;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DSerfozo.RpcBindings.Json.Tests
{
    public class JsonRpcBindingHostTests
    {
        [Fact]
        public void RepositoryDisposed()
        {
            var connection = Mock.Of<IConnection<JToken>>();

            IBindingRepository bindingRepository;
            using (var host = new JsonRpcBindingHost(js => connection))
            {
                bindingRepository = host.Repository;
            }

            Assert.Throws<ObjectDisposedException>(() => bindingRepository.AddBinding("sdf", new object()));
        }
    }
}
