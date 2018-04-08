using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Execution.Model
{
    [DataContract]
    public class CallbackResult<TMarshal>
    {
        [DataMember]
        public long ExecutionId { get; set; }

        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public TMarshal Result { get; set; }

        [DataMember]
        public bool Success { get; set; }
    }
}
