using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class PropertySetExecution<TMarshal>
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public int PropertyId { get; set; }

        [ShouldSerialize]
        public int ObjectId { get; set; }

        [ShouldSerialize]
        public TMarshal Value { get; set; }
    }
}
