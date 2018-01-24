using DSerfozo.RpcBindings.Communication;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Contract
{
    public interface ICallbackExecutor
    {
        Task<object> Execute(int id, object[] args);

        void DeleteCallback(int id);
    }
}