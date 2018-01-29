using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class CallbackExecution<TMarshal>
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public int FunctionId { get; set; }

        [ShouldSerialize]
        public TMarshal[] Parameters { get; set; }
    }
}
