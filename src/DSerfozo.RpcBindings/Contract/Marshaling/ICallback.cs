using System;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Contract
{
    public interface ICallback : IDisposable
    {
        bool CanExecute { get; }

        Task<object> ExecuteAsync(params object[] args);
    }
}
