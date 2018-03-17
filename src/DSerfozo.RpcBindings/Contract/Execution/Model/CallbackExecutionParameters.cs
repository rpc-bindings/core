using System;

namespace DSerfozo.RpcBindings.Contract.Model
{
    public class CallbackExecutionParameters<TMarshal>
    {
        public long Id { get; set; }

        public object[] Parameters { get; set; }

        public Type ResultTargetType { get; set; }

        public IParameterBinder<TMarshal> Binder { get; set; }
    }
}
