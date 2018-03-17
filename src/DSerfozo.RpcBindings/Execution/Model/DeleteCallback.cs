using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public class DeleteCallback
    {
        [ShouldSerialize]
        public long FunctionId { get; set; }
    }
}
