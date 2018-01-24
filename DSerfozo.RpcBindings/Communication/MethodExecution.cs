using Newtonsoft.Json;

namespace DSerfozo.RpcBindings.Model
{
    public class MethodExecution<TMarshal>
    {
        [JsonProperty("executionId")]
        public string Key { get; set; }

        [JsonProperty("objectId")]
        public int ObjectId { get; set; }

        [JsonProperty("methodId")]
        public int MethodId { get; set; }

        [JsonProperty("parameters")]
        public TMarshal[] Parameters { get; set; }
    }
}
