using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.CefGlue.Common.Serialization;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Browser
{
    public class CefValueBinder : CallbackAwareBinder<CefValue>
    {
        public CefValueBinder(ICallbackFactory<CefValue> callbackFactory) : base(callbackFactory)
        {
        }

        public override CefValue BindToWire(object obj)
        {
            return ObjectSerializer.Serialize(obj, new HashSet<object>());
        }

        protected override object BindInternal(ParameterBinding<CefValue> binding)
        {
            return ObjectSerializer.Deserialize(binding.Value, binding.TargetType);
        }

        protected override long? RetrieveFunctionId(CefValue marshal)
        {
            if (marshal.GetValueType() == CefValueType.List)
            {
                using (var cefList = marshal.GetList())
                {
                    if (cefList.Count == 2 && (CefTypes)cefList.GetInt(0) == CefTypes.Callback)
                    {
                        return cefList.GetInt64(1);
                    }
                }
            }
            return null;
        }
    }
}
