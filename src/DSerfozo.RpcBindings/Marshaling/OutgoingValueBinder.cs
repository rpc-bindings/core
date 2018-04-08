using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Analyze;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Contract.Marshaling.Model;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class OutgoingValueBinder<TMarshal>
    {
        private readonly BindingDelegate<TMarshal> next;
        private readonly IBindingRepository bindingRepository;

        public OutgoingValueBinder(BindingDelegate<TMarshal> next, IBindingRepository bindingRepository)
        {
            this.next = next;
            this.bindingRepository = bindingRepository;
        }

        public void Bind(BindingContext<TMarshal> ctx)
        {
            if (ctx.Direction == ObjectBindingDirection.Out && 
                ctx.BindValue != null &&
                ctx.ObjectValue != null)
            {
                var analyzeProperties = false;
                var extractPropertyValues = false;
                if (ctx.BindValue.ExtractPropertyValues)
                {
                    analyzeProperties = true;
                    extractPropertyValues = true;
                }
                var descriptor = bindingRepository.AddBinding(ctx.ObjectValue, new AnalyzeOptions
                {
                    AnalyzeProperties = analyzeProperties,
                    ExtractPropertyValues = extractPropertyValues
                });
                ctx.ObjectValue = descriptor;
            }

            next(ctx);
        }
    }
}
