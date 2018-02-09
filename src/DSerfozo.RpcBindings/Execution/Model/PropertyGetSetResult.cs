using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class PropertyGetSetResult<TMarshal>
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public bool Success { get; set; }

        [ShouldSerialize]
        public string Error { get; set; }

        [ShouldSerialize]
        public TMarshal Value { get; set; }
    }
}
