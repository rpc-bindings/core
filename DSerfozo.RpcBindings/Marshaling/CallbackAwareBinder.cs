using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling.Model;

namespace DSerfozo.RpcBindings.Marshaling
{
    public abstract partial class CallbackAwareBinder<TMarshal> : IParameterBinder<TMarshal>
    {
        private readonly ICallbackFactory callbackFactory;

        public CallbackAwareBinder(ICallbackFactory callbackFactory)
        {
            this.callbackFactory = callbackFactory;
        }

        public object BindToNet(ParameterBinding<TMarshal> binding)
        {
            object result = null;

            if(binding.TargetType == typeof(ICallback))
            {
                var callbackParam = CreateCallbackParameter(binding.Value);
                if (callbackParam != null && callbackParam.Id > 0)
                {
                    result = callbackFactory.Create(callbackParam.Id);
                }
            }
            else
            {
                result = BindInternal(binding);
            }

            return result;
        }

        public abstract TMarshal BindToWire(object obj);

        protected abstract object BindInternal(ParameterBinding<TMarshal> binding);

        protected abstract CallbackParameter CreateCallbackParameter(TMarshal marshal);
    }
}
