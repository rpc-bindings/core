using System;

namespace DSerfozo.RpcBindings.Contract
{
    public interface ICallbackFactory<TMarshal>
    {
        object CreateCallback(long id, Type delegateType, IParameterBinder<TMarshal> parameterBinder);
    }
}
