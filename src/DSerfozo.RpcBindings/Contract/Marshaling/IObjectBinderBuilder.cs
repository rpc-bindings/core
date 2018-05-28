using System;

namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    public interface IObjectBinderBuilder<TMarshal>
    {
        IObjectBinderBuilder<TMarshal> Use(Func<BindingDelegate<TMarshal>, BindingDelegate<TMarshal>> binder);
        BindingDelegate<TMarshal> Build();
    }
}