using Newtonsoft.Json;

namespace DSerfozo.RpcBindings.Marshaling.Model
{
    public class CallbackParameter
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
