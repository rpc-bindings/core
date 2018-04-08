using System;

namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    public class Binding<T>
    {
        public Type TargetType { get; set; }

        public T Value { get; set; }
    }
}
