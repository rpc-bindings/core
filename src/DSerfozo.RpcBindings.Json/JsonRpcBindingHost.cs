using System;
using System.Reactive.Concurrency;
using DSerfozo.RpcBindings.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSerfozo.RpcBindings.Json
{
    public class JsonRpcBindingHost : RpcBindingHost<JToken>
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer
        {
            ContractResolver = new ShouldSerializeContractResolver()
        };

        public JsonRpcBindingHost(Func<JsonSerializer, IConnection<JToken>> connectionFactory)
            :base(connectionFactory(JsonSerializer), new JsonBinder(JsonSerializer), new EventLoopScheduler())
        {
            
        }
    }
}
