using DSerfozo.RpcBindings.Communication;
using Newtonsoft.Json;

namespace DSerfozo.RpcBindings.Model
{
    public class RpcRequest<TMarshal>
    {
        [JsonProperty("methodResult")]
        public MethodResult MethodResult { get; set; }

        [JsonProperty("callbackExecution")]
        public CallbackExecution<TMarshal> CallbackExecution { get; set; }

        [JsonProperty("deleteCallback")]
        public DeleteCallback DeleteCallback { get; set; }
    }
}
