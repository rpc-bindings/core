using DSerfozo.RpcBindings.Model;
using Newtonsoft.Json;

namespace DSerfozo.RpcBindings.Communication
{
    public class RpcResponse<TMarshal>
    {
        [JsonProperty("methodExecution")]
        public MethodExecution<TMarshal> MethodExecution { get; set; }

        [JsonProperty("callbackResult")]
        public CallbackResult<TMarshal> CallbackResult { get; set; }
    }
}
