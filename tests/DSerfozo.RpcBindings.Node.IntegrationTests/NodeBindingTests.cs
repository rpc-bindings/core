using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Json;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.NodeServices.Sockets;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DSerfozo.RpcBindings.Node.IntegrationTests
{
    public class NodeBindingTests : IDisposable
    {
        private readonly CancellationTokenSource stopTokenSource = new CancellationTokenSource();
        private readonly INodeServices nodeServices;
        private readonly LineDelimitedJsonConnection connection;
        private readonly JsonRpcBindingHost rpcHost;
        private readonly Task initTask;

        public class TestBound
        {
            public int TestMethod1(int input)
            {
                return input+1;
            }

            public async Task<string> DelegateTest(Func<string, Task<string>> func)
            {
                return await func("test").ConfigureAwait(false);
            }
        }

        public NodeBindingTests()
        {
            var options = new NodeServicesOptions
            {
                ApplicationStoppingToken = stopTokenSource.Token,
                ProjectPath = AppDomain.CurrentDomain.BaseDirectory,
                NodeInstanceOutputLogger = Mock.Of<ILogger>(),
                //DebuggingPort = 8989,
                //LaunchWithDebugging = true
            };
            options.UseSocketHosting();
            nodeServices = NodeServicesFactory.CreateNodeServices(options);

            connection = new LineDelimitedJsonConnection(new JsonSerializer()
            {
                ContractResolver = new ShouldSerializeContractResolver()
            });
            rpcHost = new JsonRpcBindingHost(connection);
            rpcHost.Repository.AddBinding("test", new TestBound());

            var initTask = nodeServices.InvokeExportAsync<Stream>("binding-init", "initialize", rpcHost.Repository.Objects);
            this.initTask = initTask.ContinueWith(t => connection.Initialize(t.Result, t.Result));

        }

        [Fact]
        public async Task SimpleMethodCallWorks()
        {
            await initTask;

            var result = await nodeServices.InvokeExportAsync<int>("binding-test", "testMethod1", 2);

            Assert.Equal(3, result);
        }

        [Fact]
        public async Task DelegateCallbackBindingWorks()
        {
            await initTask;

            var result = await nodeServices.InvokeExportAsync<string>("binding-test", "delegateTest");
            
            Assert.Equal("test->JS", result);
        }

        public void Dispose()
        {
            stopTokenSource.Cancel();
            nodeServices.Dispose();
        }
    }
}
