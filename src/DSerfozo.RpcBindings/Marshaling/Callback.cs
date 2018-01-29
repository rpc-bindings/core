using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class Callback<TMarshal> : CallbackBase<TMarshal>, ICallback
    {
        public Callback(int id, ICallbackExecutor<TMarshal> executor, IParameterBinder<TMarshal> parameterBinder) 
            : base(id, executor, parameterBinder)
        {
        }
    }
}
