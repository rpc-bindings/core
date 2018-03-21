using DSerfozo.RpcBindings.Contract;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Marshaling.Delegates
{
    public class DelegateContainerBase<TMarshal> : CallbackBase<TMarshal>
    {
        public DelegateContainerBase(long id, ICallbackExecutor<TMarshal> executor, BindingDelegate<TMarshal> parameterBinder) 
            : base(id, executor, parameterBinder)
        {
        }

        protected async Task<T> ExecuteAsync<T>(params object[] obj)
        {
            return (T)await ExecuteAsync(obj).ConfigureAwait(false);
        }

        ~DelegateContainerBase()
        {
            Dispose();
        }
    }
}
