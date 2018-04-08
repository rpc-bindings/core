using System.Threading.Tasks;
using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Execution
{
    public interface IMethodExecutor<TMarshal>
    {
        Task<MethodResult<TMarshal>> Execute(MethodExecution<TMarshal> methodExcecution);
    }
}