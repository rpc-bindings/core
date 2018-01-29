using System;

namespace DSerfozo.RpcBindings.Contract
{
    public class ParameterBinding<T>
    {
        public Type TargetType { get; set; }

        public T Value { get; set; }
    }
}
