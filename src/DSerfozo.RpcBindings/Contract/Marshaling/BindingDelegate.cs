using DSerfozo.RpcBindings.Contract.Marshaling.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public delegate void BindingDelegate<TMarshal>(BindingContext<TMarshal> context);
}
