using System.Threading.Tasks;
using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IMethodExecutor<TMarshal>
    {
        Task<MethodResult<TMarshal>> Execute(MethodExecution<TMarshal> methodExcecution);
    }
}