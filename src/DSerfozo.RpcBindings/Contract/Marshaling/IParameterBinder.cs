namespace DSerfozo.RpcBindings.Contract
{
    public interface IParameterBinder<T>
    {
        object BindToNet(ParameterBinding<T> binding);

        T BindToWire(object obj);
    }
}
