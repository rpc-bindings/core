using System.Runtime.Serialization;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    [DataContract]
    public class DynamicObjectResponse
    {
        [DataMember]
        public long ExecutionId { get; set; }

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Exception { get; set; }

        [DataMember]
        public ObjectDescriptor ObjectDescriptor { get; set; }
    }
}
