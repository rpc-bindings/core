using System;
using DSerfozo.RpcBindings.Marshaling;

namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    public interface IObjectBinderBuilder<TMarshal>
    {
        ObjectBinderBuilder<TMarshal> Use(Func<BindingDelegate<TMarshal>, BindingDelegate<TMarshal>> binder);
        BindingDelegate<TMarshal> Build();
    }
}