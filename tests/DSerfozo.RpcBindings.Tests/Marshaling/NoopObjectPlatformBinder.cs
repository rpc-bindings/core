using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class NoopObjectPlatformBinder : IPlatformBinder<object>
    {
        public object BindToNet(Binding<object> binding)
        {
            return binding.Value;
        }

        public object BindToWire(object obj)
        {
            return obj;
        }
    }
}
