using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Contract.Execution.Model
{
    public class CallbackParameter
    {
        public object Value { get; set; }

        public BindValueAttribute Bindable { get; set; }
    }
}
