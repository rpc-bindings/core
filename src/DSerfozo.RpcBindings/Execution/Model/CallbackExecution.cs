using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class CallbackExecution<TMarshal>
    {
        [ShouldSerialize]
        public long ExecutionId { get; set; }

        [ShouldSerialize]
        public long FunctionId { get; set; }

        [ShouldSerialize]
        public TMarshal[] Parameters { get; set; }
    }
}
