namespace DSerfozo.RpcBindings.Contract
{
    public interface ICallbackFactory
    {
        ICallback Create(int id);
    }
}
