using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class PlatformBinder<TMarshal>
    {
        private readonly IPlatformBinder<TMarshal> marshal;

        public PlatformBinder(BindingDelegate<TMarshal> next, IPlatformBinder<TMarshal> marshal)
        {
            this.marshal = marshal;
        }

        public void Bind(BindingContext<TMarshal> ctx)
        {
            if (ctx.Direction == ObjectBindingDirection.In)
            {
                ctx.ObjectValue = marshal.BindToNet(new Binding<TMarshal>
                {
                    TargetType = ctx.TargetType,
                    Value = ctx.NativeValue
                });
            }
            else if(ctx.ObjectValue != null)
            {
                ctx.NativeValue = marshal.BindToWire(ctx.ObjectValue);
            }
        }
    }
}
