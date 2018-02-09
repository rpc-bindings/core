using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    public class RpcResponse<TMarshal>
    {
        [ShouldSerialize]
        public MethodExecution<TMarshal> MethodExecution { get; set; }

        [ShouldSerialize]
        public CallbackResult<TMarshal> CallbackResult { get; set; }

        [ShouldSerialize]
        public PropertyGetExecution PropertyGet { get; set; }

        [ShouldSerialize]
        public PropertySetExecution<TMarshal> PropertySet { get; set; }
    }
}
