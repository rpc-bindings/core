using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Execution.Model
{
    [DataContract]
    public class CallbackExecution<TMarshal>
    {
        [DataMember]
        public long ExecutionId { get; set; }

        [DataMember]
        public long FunctionId { get; set; }

        [DataMember]
        public TMarshal[] Parameters { get; set; }
    }
}
