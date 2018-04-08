using System;
using System.Linq;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Execution.Model;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Marshaling
{
    public abstract class CallbackBase<TMarshal> : IDisposable
    {
        private readonly long id;
        private readonly ICallbackExecutor<TMarshal> executor;
        private readonly BindingDelegate<TMarshal> parameterBinder;
        private readonly Type resultType;
        private bool disposed;

        public bool CanExecute => !disposed && executor.CanExecute;

        public long Id => id;

        protected CallbackBase(long id, ICallbackExecutor<TMarshal> executor, BindingDelegate<TMarshal> parameterBinder,
            Type resultType)
        {
            this.id = id;
            this.executor = executor;
            this.parameterBinder = parameterBinder;
            this.resultType = resultType;
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
                Parameters = args.Zip(GetBindables(args), (b, p) => new CallbackParameter
                {
                    Value = b,
                    Bindable = p
                }).ToArray(),
                ResultTargetType = resultType
            }).ConfigureAwait(false);
        }

        protected virtual BindValueAttribute[] GetBindables(object[] args)
        {
            return args.Select(_ => (BindValueAttribute)null).ToArray();
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
