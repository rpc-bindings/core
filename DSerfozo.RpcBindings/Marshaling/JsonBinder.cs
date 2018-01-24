using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class JsonBinder : CallbackAwareBinder<JToken>
    {
        public JsonBinder(ICallbackFactory callbackFactory) : base(callbackFactory)
        {
        }

        public override JToken BindToWire(object obj)
        {
            return JToken.FromObject(obj);
        }

        protected override object BindInternal(ParameterBinding<JToken> binding)
        {
            object result = null;
            var val = binding.Value;

            if (binding.TargetType != null)
            {
                result = val.ToObject(binding.TargetType);
            }
            else
            {
                if(val?.Type == JTokenType.Object)
                {
                    result = val.ToObject<IDictionary<string, object>>();
                }
                else
                {
                    result = val?.ToObject<object>();
                }
            }

            return result;
        }

        protected override CallbackParameter CreateCallbackParameter(JToken marshal)
        {
            return marshal.ToObject<CallbackParameter>();
        }
    }
}
