using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Execution.Model
{
    [DataContract]
    public class MethodExecution<TMarshal>
    {
        [DataMember]
        public long ExecutionId { get; set; }

        [DataMember]
        public long ObjectId { get; set; }

        [DataMember]
        public long MethodId { get; set; }

        [DataMember]
        public TMarshal[] Parameters { get; set; }
    }
}
