using System.Runtime.Serialization;
using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    [DataContract]
    public class RpcResponse<TMarshal>
    {
        [DataMember]
        public MethodExecution<TMarshal> MethodExecution { get; set; }

        [DataMember]
        public CallbackResult<TMarshal> CallbackResult { get; set; }

        [DataMember]
        public PropertyGetExecution PropertyGet { get; set; }

        [DataMember]
        public PropertySetExecution<TMarshal> PropertySet { get; set; }

        [DataMember]
        public DynamicObjectRequest DynamicObjectRequest { get; set; }
    }
}
