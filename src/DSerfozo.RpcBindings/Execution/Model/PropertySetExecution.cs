using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Execution.Model
{
    [DataContract]
    public class PropertySetExecution<TMarshal>
    {
        [DataMember]
        public int ExecutionId { get; set; }

        [DataMember]
        public int PropertyId { get; set; }

        [DataMember]
        public int ObjectId { get; set; }

        [DataMember]
        public TMarshal Value { get; set; }
    }
}
