using System.Runtime.Serialization;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    [DataContract]
    [TypeId(TypeId)]
    public class RpcRequest<TMarshal>
    {
        public const string TypeId = "{CA41E1C5-5BF3-4ACE-B342-CF0A0C46A934}";

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
