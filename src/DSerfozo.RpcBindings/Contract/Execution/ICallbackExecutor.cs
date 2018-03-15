using System;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract.Model;
using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface ICallbackExecutor<TMarshal>: IObservable<CallbackExecution<TMarshal>>, IObservable<DeleteCallback>
    {
        bool CanExecute { get; }

        Task<object> Execute(CallbackExecutionParameters<TMarshal> execute);

        void DeleteCallback(int id);
    }
}