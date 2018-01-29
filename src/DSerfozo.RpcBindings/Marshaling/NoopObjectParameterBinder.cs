using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class NoopObjectParameterBinder : IParameterBinder<object>
    {
        public object BindToNet(ParameterBinding<object> binding)
        {
            return binding.Value;
        }

        public object BindToWire(object obj)
        {
            return obj;
        }
    }
}
