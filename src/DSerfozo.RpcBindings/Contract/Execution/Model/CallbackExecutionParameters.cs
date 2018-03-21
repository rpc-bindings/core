using System;

namespace DSerfozo.RpcBindings.Contract.Model
{
    public class CallbackExecutionParameters<TMarshal> : IBinder<TMarshal>
    {
        public long Id { get; set; }

        public object[] Parameters { get; set; }

        public Type ResultTargetType { get; set; }

        public BindingDelegate<TMarshal> Binder { get; set; }
    }
}
