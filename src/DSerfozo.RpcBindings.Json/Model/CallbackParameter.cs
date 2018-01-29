using Newtonsoft.Json;

namespace DSerfozo.RpcBindings.Json.Model
{
    public class CallbackParameter
    {
        [JsonProperty("functionId")]
        public int FunctionId { get; set; }
    }
}
