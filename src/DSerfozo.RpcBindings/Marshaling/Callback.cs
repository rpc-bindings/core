using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class Callback<TMarshal> : CallbackBase<TMarshal>, ICallback
    {
        public Callback(long id, ICallbackExecutor<TMarshal> executor, BindingDelegate<TMarshal> parameterBinder) 
            : base(id, executor, parameterBinder, typeof(object))
        {
        }
    }
}
