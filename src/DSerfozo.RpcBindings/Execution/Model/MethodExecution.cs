using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class MethodExecution<TMarshal>
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public int ObjectId { get; set; }

        [ShouldSerialize]
        public int MethodId { get; set; }

        [ShouldSerialize]
        public TMarshal[] Parameters { get; set; }
    }
}
