using System.Runtime.Serialization;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    [DataContract]
    [TypeId(TypeId)]
    public class RpcResponse<TMarshal>
    {
        public const string TypeId = "{EEDB4DFB-C73F-4106-865E-F468005F9686}";

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
