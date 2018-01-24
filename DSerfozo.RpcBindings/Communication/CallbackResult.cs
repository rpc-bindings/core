using Newtonsoft.Json;

namespace DSerfozo.RpcBindings.Communication
{
    public class CallbackResult<TMarshal>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("result")]
        public TMarshal Result { get; set; }

        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
    }
}
