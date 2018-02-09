using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class PropertyGetExecution
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public int PropertyId { get; set; }

        [ShouldSerialize]
        public int ObjectId { get; set; }
    }
}
