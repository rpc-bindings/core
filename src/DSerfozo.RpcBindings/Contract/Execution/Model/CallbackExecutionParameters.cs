using System;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Contract.Execution.Model
{
    public class CallbackExecutionParameters<TMarshal> : IBinder<TMarshal>
    {
        public long Id { get; set; }

        public CallbackParameter[] Parameters { get; set; }

        public Type ResultTargetType { get; set; }

        public BindingDelegate<TMarshal> Binder { get; set; }
    }
}
