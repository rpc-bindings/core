using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Contract.Marshaling.Model
{
    [DataContract]
    public class CallbackDescriptor
    {
        public const string TypeId = "6412774B-E8B7-4B29-B699-877A532C9708";

        [DataMember]
        public string Type { get; set; } = TypeId;

        [DataMember]
        public long FunctionId { get; set; }
    }
}
