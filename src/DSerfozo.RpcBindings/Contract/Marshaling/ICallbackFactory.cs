using System;

namespace DSerfozo.RpcBindings.Contract
{
    public interface ICallbackFactory<TMarshal>
    {
        object CreateCallback(int id, Type delegateType, IParameterBinder<TMarshal> parameterBinder);
    }
}
