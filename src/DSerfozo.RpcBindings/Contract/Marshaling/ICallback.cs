using System;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Contract
{
    public interface ICallback : IDisposable
    {
        bool CanExecute { get; }

        Task<object> Execute(params object[] args);
    }
}
