using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Json;
using Newtonsoft.Json;

namespace DSerfozo.JavaScriptServices.Example
{
    public class BoundObject
    {
        public string DoIt()
        {
            return "Hello World from .NET!";
        }

        public async void CallbackTest(ICallback callback)
        {
            await callback.Execute("TESTMESSAGE");
        }

        public int Add(int one, int two)
        {
            return one + two;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            //var rpcBindingHost = new JsonRpcBindingHost(new LineDelimitedJsonConnection(new JsonSerializer()
            //{
            //    ContractResolver = new ShouldSerializeContractResolver()
            //}))

            //var options = new ModuleNodeServicesOptions()
            //{
            //    ApplicationStoppingToken = CancellationToken.None,
            //    ProjectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            //    NodeInstanceOutputLogger = new ConsoleLogger("logger", (_, __) => true, false),
            //    BoundObjects = new Dictionary<string, object> { { "bound", new BoundObject() }, { "bound2", new BoundObject() } },
            //    BuiltInModules = new Dictionary<string, string> { { "testmodule", "exports.do = function(one, two) { return one + two; }" } }
            //};
            //var nodeServices = new ModuleNodeServices(options);
            //await nodeServices.Initialize();

            //Console.ReadLine();

            //Console.WriteLine(await nodeServices.InvokeExportAsync<string>("test", "doIt"));

            //Console.ReadLine();
        }
    }
}
