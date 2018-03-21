using DSerfozo.RpcBindings.Contract;
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

        public static TMarshal BindToWire<TMarshal>(this IBinder<TMarshal> @this, object value)
        {
            var ctx = new BindingContext<TMarshal>(ObjectBindingDirection.Out, @this.Binder)
            {
                ObjectValue = value
            };

            @this.Binder(ctx);

            return ctx.NativeValue;
        }
    }
}
