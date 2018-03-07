using System;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Model;

namespace DSerfozo.RpcBindings.Marshaling
{
    public abstract class CallbackBase<TMarshal> : IDisposable
    {
        private readonly int id;
        private readonly ICallbackExecutor<TMarshal> executor;
        private readonly IParameterBinder<TMarshal> parameterBinder;
        private bool disposed;

        public bool CanExecute => !disposed && executor.CanExecute;

        public int Id => id;

        protected CallbackBase(int id, ICallbackExecutor<TMarshal> executor, IParameterBinder<TMarshal> parameterBinder)
        {
            this.id = id;
            this.executor = executor;
            this.parameterBinder = parameterBinder;
        }

        public async Task<object> ExecuteAsync(params object[] args)
        {
            if (!CanExecute)
            {
                throw new ObjectDisposedException(nameof(Callback<TMarshal>));
            }

            return await executor.Execute(new CallbackExecutionParameters<TMarshal>
            {
                Binder = parameterBinder,
                Id = id,
                Parameters = args,
                ResultTargetType = typeof(object)
            }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (executor.CanExecute)
                {
                    executor.DeleteCallback(id);
                }

                disposed = true;
            }
        }
    }
}
