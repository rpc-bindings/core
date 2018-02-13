namespace DSerfozo.RpcBindings.Contract.Communication.Model
{
    public class DynamicObjectRequest
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public string Name { get; set; }
    }
}
