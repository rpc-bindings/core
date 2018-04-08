using System.Runtime.Serialization;
using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    [DataContract]
    public class RpcRequest<TMarshal>
    {
        [DataMember]
        public MethodResult<TMarshal> MethodResult { get; set; }

        [DataMember]
        public CallbackExecution<TMarshal> CallbackExecution { get; set; }

        [DataMember]
        public DeleteCallback DeleteCallback { get; set; }

        [DataMember]
        public PropertyGetSetResult<TMarshal> PropertyResult { get; set; }

        [DataMember]
        public DynamicObjectResponse DynamicObjectResult { get; set; }
    }
}
