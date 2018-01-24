using Newtonsoft.Json;
using System;

namespace DSerfozo.RpcBindings.Model
{
    public sealed class MethodResult
    {
        [JsonProperty("executionId")]
        public string Key { get; set; }

        [JsonProperty("success")]
        public bool IsSuccess { get; set; }

        [JsonProperty("result")]
        public object Result { get; set; }

        [JsonProperty("error")]
        public Exception Error { get; set; }
    }
}
