using System;

namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    public interface ICallbackFactory<TMarshal>
    {
        object CreateCallback(long id, Type delegateType, BindingDelegate<TMarshal> parameterBinder);
    }
}
