namespace DSerfozo.RpcBindings.Contract
{
    public interface IPlatformBinder<T>
    {
        object BindToNet(Binding<T> binding);

        T BindToWire(object obj);
    }
}
