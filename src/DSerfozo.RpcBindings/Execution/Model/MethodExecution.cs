using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class MethodExecution<TMarshal>
    {
        [ShouldSerialize]
        public long ExecutionId { get; set; }

        [ShouldSerialize]
        public long ObjectId { get; set; }

        [ShouldSerialize]
        public long MethodId { get; set; }

        [ShouldSerialize]
        public TMarshal[] Parameters { get; set; }
    }
}
