using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    public class RpcResponse<TMarshal>
    {
        [ShouldSerialize]
        public MethodExecution<TMarshal> MethodExecution { get; set; }

        [ShouldSerialize]
        public CallbackResult<TMarshal> CallbackResult { get; set; }
    }
}
