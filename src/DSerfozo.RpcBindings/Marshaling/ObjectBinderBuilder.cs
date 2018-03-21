using System;
using System.Collections.Generic;
using System.Linq;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class ObjectBinderBuilder<TMarshal> : IObjectBinderBuilder<TMarshal>
    {
        private readonly IList<Func<BindingDelegate<TMarshal>, BindingDelegate<TMarshal>>> binderComponents =
            new List<Func<BindingDelegate<TMarshal>, BindingDelegate<TMarshal>>>();

        public ObjectBinderBuilder<TMarshal> Use(Func<BindingDelegate<TMarshal>, BindingDelegate<TMarshal>> binder)
        {
            binderComponents.Add(binder);
            return this;
        }

        public BindingDelegate<TMarshal> Build()
        {
            return binderComponents
                .Reverse()
                .Aggregate(new BindingDelegate<TMarshal>(context => { }),
                    (current, binderComponent) => binderComponent(current));
        }
    }
}
