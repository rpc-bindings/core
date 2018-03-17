using DSerfozo.RpcBindings.Contract;
using System;

namespace DSerfozo.RpcBindings.Marshaling
{
    public abstract class CallbackAwareBinder<TMarshal> : IParameterBinder<TMarshal>
    {
        private readonly ICallbackFactory<TMarshal> callbackFactory;

        protected CallbackAwareBinder(ICallbackFactory<TMarshal> callbackFactory)
        {
            this.callbackFactory = callbackFactory;
        }

        public object BindToNet(ParameterBinding<TMarshal> binding)
        {
            object result = null;

            var isTypedCallback = binding.TargetType != null && typeof(MulticastDelegate).IsAssignableFrom(binding.TargetType);
            var isCallback = binding.TargetType == typeof(ICallback);
            if (isTypedCallback || isCallback)
            {
                var callbackParam = RetrieveFunctionId(binding.Value);
                if (callbackParam.HasValue && callbackParam > 0)
                {
                    result = CreateCallback(isTypedCallback ? binding.TargetType : null, callbackParam.Value);
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

        protected abstract long? RetrieveFunctionId(TMarshal marshal);

        private object CreateCallback(Type delegateType, long functionId)
        {
            return callbackFactory.CreateCallback(functionId, delegateType, this);
        }
    }
}
