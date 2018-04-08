using System;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    public interface ICallback : IDisposable
    {
        bool CanExecute { get; }

        Task<object> ExecuteAsync(params object[] args);
    }
}
