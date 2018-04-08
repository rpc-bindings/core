using System;

namespace DSerfozo.RpcBindings.Contract.Marshaling.Model
{
    public class BindingContext<TMarshal>
    {
        public ObjectBindingDirection Direction { get; }

        public BindingDelegate<TMarshal> Binder { get; }

        public BindValueAttribute BindValue { get; set; }

        public Type TargetType { get; set; }

        public TMarshal NativeValue { get; set; }

        public object ObjectValue { get; set; }

        public BindingContext(ObjectBindingDirection direction, BindingDelegate<TMarshal> binder)
        {
            Direction = direction;
            Binder = binder;
        }
    }
}
