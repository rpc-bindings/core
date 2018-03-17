using System.Collections.Generic;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Json.Model;
using DSerfozo.RpcBindings.Marshaling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSerfozo.RpcBindings.Json
{
    public class JsonBinder : CallbackAwareBinder<JToken>
    {
        private readonly JsonSerializer serializer;

        public JsonBinder(JsonSerializer serializer, ICallbackFactory<JToken> callbackFactory) : base(callbackFactory)
        {
            this.serializer = serializer;
        }

        public override JToken BindToWire(object obj)
        {
            return JToken.FromObject(obj, serializer);
        }

        protected override object BindInternal(ParameterBinding<JToken> binding)
        {
            object result = null;
            var val = binding.Value;

            if (binding.TargetType != null)
            {
                result = val.ToObject(binding.TargetType, serializer);
            }
            else
            {
                if(val?.Type == JTokenType.Object)
                {
                    result = val.ToObject<IDictionary<string, object>>(serializer);
                }
                else
                {
                    result = val?.ToObject<object>(serializer);
                }
            }

            return result;
        }

        protected override long? RetrieveFunctionId(JToken marshal)
        {
            return marshal.ToObject<CallbackParameter>(serializer).FunctionId;
        }
    }
}
