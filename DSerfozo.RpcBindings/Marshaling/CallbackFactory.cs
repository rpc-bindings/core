using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class CallbackFactory : ICallbackFactory
    {
        public ICallbackExecutor CallbackExecutor { get; set; }


        public ICallback Create(int id)
        {
            return new Callback(id, CallbackExecutor);
        }
    }
}
