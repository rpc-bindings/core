using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class DeleteCallback
    {
        [ShouldSerialize]
        public int FunctionId { get; set; }
    }
}
