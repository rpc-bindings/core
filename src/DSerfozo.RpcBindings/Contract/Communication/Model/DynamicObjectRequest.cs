using System.Runtime.Serialization;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    [DataContract]
    public class DynamicObjectRequest
    {
        [DataMember]
        public long ExecutionId { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
