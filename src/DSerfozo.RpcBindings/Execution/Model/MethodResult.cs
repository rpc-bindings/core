using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public sealed class MethodResult<TMarshal>
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public bool Success { get; set; }

        [ShouldSerialize]
        public TMarshal Result { get; set; }

        [ShouldSerialize]
        public string Error { get; set; }
    }
}
