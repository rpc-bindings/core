using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    public class DynamicObjectResponse
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public bool Success { get; set; }

        [ShouldSerialize]
        public string Exception { get; set; }

        [ShouldSerialize]
        public ObjectDescriptor ObjectDescriptor { get; set; }
    }
}
