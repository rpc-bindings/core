using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Execution.Model
{
    [DataContract]
    public class PropertyGetSetResult<TMarshal>
    {
        [DataMember]
        public int ExecutionId { get; set; }

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public TMarshal Value { get; set; }
    }
}
