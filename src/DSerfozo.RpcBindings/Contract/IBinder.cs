namespace DSerfozo.RpcBindings.Contract
{
    public interface IBinder<TMarshal>
    {
        BindingDelegate<TMarshal> Binder { get; }
    }
}
