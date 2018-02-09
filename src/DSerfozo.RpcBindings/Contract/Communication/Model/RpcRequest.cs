using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    public class RpcRequest<TMarshal>
    {
        [ShouldSerialize]
        public MethodResult MethodResult { get; set; }

        [ShouldSerialize]
        public CallbackExecution<TMarshal> CallbackExecution { get; set; }

        [ShouldSerialize]
        public DeleteCallback DeleteCallback { get; set; }

        [ShouldSerialize]
        public PropertyGetSetResult<TMarshal> PropertyResult { get; set; }
    }
}
