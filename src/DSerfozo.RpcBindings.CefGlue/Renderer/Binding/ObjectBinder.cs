using System.Collections.Generic;
using System.Linq;
using DSerfozo.RpcBindings.CefGlue.Renderer.Serialization;
using DSerfozo.RpcBindings.CefGlue.Renderer.Util;
using DSerfozo.RpcBindings.Model;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Renderer.Binding
{
    public class ObjectBinder
    {
        private readonly IDictionary<long, FunctionBinder> functions;

        public ObjectBinder(ObjectDescriptor descriptor, V8Serializer v8Serializer, SavedValueFactory<Promise> functionCallRegistry)
        {
            functions = descriptor.Methods?.Select(m => new {m.Key, Value = new FunctionBinder(descriptor.Id, m.Value, v8Serializer, functionCallRegistry)})
                .ToDictionary(k => k.Key, v => v.Value);
        }

        public CefV8Value BindToNew()
        {
            var obj = CefV8Value.CreateObject();

            functions?.Values.ToList().ForEach(m => m.Bind(obj));

            return obj;
        }
    }
}
