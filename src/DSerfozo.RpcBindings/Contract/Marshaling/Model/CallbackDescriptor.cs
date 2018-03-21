namespace DSerfozo.RpcBindings.Contract.Model
{
    public class CallbackDescriptor
    {
        [ShouldSerialize]
        public long FunctionId { get; set; }
    }
}
