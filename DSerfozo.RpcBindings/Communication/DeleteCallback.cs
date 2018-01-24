using Newtonsoft.Json;

namespace DSerfozo.RpcBindings.Communication
{
    public class DeleteCallback
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
