using Newtonsoft.Json;

namespace DSerfozo.RpcBindings.Model
{
    public class CallbackExecution<TMarshal>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("parameters")]
        public TMarshal[] Parameters { get; set; }
    }
}
