using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;

namespace DSerfozo.RpcBindings.Extensions
{
    public static class BinderExtensions
    {
        public static object BindToNet<TMarshal>(this IBinder<TMarshal> @this, Binding<TMarshal> obj)
        {
            var ctx = new BindingContext<TMarshal>(ObjectBindingDirection.In, @this.Binder)
            {
                TargetType = obj.TargetType,
                NativeValue = obj.Value
            };

            @this.Binder(ctx);

            return ctx.ObjectValue;
        }

        public static TMarshal BindToWire<TMarshal>(this IBinder<TMarshal> @this, object value, BindValueAttribute bindValue)
        {
            var ctx = new BindingContext<TMarshal>(ObjectBindingDirection.Out, @this.Binder)
            {
                ObjectValue = value,
                BindValue = bindValue
            };

            @this.Binder(ctx);

            return ctx.NativeValue;
        }
    }
}
