using DSerfozo.RpcBindings.Contract.Marshaling.Model;

namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    public delegate void BindingDelegate<TMarshal>(BindingContext<TMarshal> context);
}
