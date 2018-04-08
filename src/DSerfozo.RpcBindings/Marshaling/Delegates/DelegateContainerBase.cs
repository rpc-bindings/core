using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Marshaling.Delegates
{
    public class DelegateContainerBase<TMarshal> : CallbackBase<TMarshal>
    {
        private readonly BindValueAttribute[] bindValues;

        public DelegateContainerBase(long id, ICallbackExecutor<TMarshal> executor, BindingDelegate<TMarshal> parameterBinder, Type resultType, Type delegateType) 
            : base(id, executor, parameterBinder, resultType)
        {
            bindValues = delegateType.GetMethod(nameof(Action.Invoke)).GetParameters()
                .Select(p => p.GetCustomAttribute<BindValueAttribute>()).ToArray();
        }

        protected async Task<T> ExecuteAsync<T>(params object[] obj)
        {
            return (T)await ExecuteAsync(obj).ConfigureAwait(false);
        }

        protected override BindValueAttribute[] GetBindables(object[] args)
        {
            return base.GetBindables(args).Zip(bindValues, (_, __) => _ ?? __).ToArray();
        }

        ~DelegateContainerBase()
        {
            Dispose();
        }
    }
}
