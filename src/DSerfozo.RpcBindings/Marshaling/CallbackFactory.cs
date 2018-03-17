using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling.Delegates;
using System;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class CallbackFactory<TMarshal> : ICallbackFactory<TMarshal>
    {
        private readonly CallbackDelegateGenerator<TMarshal> generator = new CallbackDelegateGenerator<TMarshal>();
        private readonly ICallbackExecutor<TMarshal> callbackExecutor;

        public CallbackFactory(ICallbackExecutor<TMarshal> callbackExecutor)
        {
            this.callbackExecutor = callbackExecutor;
        }

        public object CreateCallback(long id, Type delegateType, IParameterBinder<TMarshal> parameterBinder)
        {
            if(delegateType == null)
            {
                return Create(id, parameterBinder);
            }
            else
            {
                return Create(id, delegateType, parameterBinder);
            }
        }

        private Delegate Create(long id, Type delegateType, IParameterBinder<TMarshal> parameterBinder)
        {
            return generator.Generate(delegateType, id, callbackExecutor, parameterBinder);
        }

        private ICallback Create(long id, IParameterBinder<TMarshal> parameterBinder)
        {
            return new Callback<TMarshal>(id, callbackExecutor, parameterBinder);
        }
    }
}
