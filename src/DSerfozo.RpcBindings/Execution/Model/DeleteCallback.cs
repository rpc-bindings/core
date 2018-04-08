using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Execution.Model
{
    [DataContract]
    public class DeleteCallback
    {
        [DataMember]
        public long FunctionId { get; set; }
    }
}
