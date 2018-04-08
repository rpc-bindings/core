using DSerfozo.RpcBindings.Execution.Model;

namespace DSerfozo.RpcBindings.Contract.Execution
{
    public interface IPropertyExecutor<TMarshal>
    {
        PropertyGetSetResult<TMarshal> Execute(PropertyGetExecution propertyGetExecution);
        PropertyGetSetResult<TMarshal> Execute(PropertySetExecution<TMarshal> propertySetExecution);
    }
}