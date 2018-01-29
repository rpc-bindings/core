using System.Threading.Tasks;
using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IMethodExecutor<TMarshal>
    {
        Task<MethodResult> Execute(MethodExecution<TMarshal> methodExcecution);
    }
}