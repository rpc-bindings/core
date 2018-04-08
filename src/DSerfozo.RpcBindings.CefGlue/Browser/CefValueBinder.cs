using System.Collections.Generic;
using DSerfozo.RpcBindings.CefGlue.Common.Serialization;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Browser
{
    public class CefValueBinder : IPlatformBinder<CefValue>
    {
        public CefValue BindToWire(object obj)
        {
            return ObjectSerializer.Serialize(obj, new HashSet<object>());
        }

        public object BindToNet(Binding<CefValue> binding)
        {
            return ObjectSerializer.Deserialize(binding.Value, binding.TargetType);
        }
    }
}
