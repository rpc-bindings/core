namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    public interface IPlatformBinder<T>
    {
        object BindToNet(Binding<T> binding);

        T BindToWire(object obj);
    }
}
