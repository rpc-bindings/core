using System.Collections.Generic;
using System.Linq;
using DSerfozo.RpcBindings.CefGlue.Renderer.Model;
using DSerfozo.RpcBindings.CefGlue.Renderer.Serialization;
using DSerfozo.RpcBindings.CefGlue.Renderer.Util;
using DSerfozo.RpcBindings.Model;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Renderer.Binding
{
    public class ObjectBinder
    {
        private readonly V8Serializer v8Serializer;
        private readonly IDictionary<long, FunctionBinder> functions;
        private readonly List<CefPropertyDescriptor> propertyDescriptors;

        public ObjectBinder(ObjectDescriptor descriptor, V8Serializer v8Serializer, SavedValueFactory<Promise> functionCallRegistry)
        {
            this.v8Serializer = v8Serializer;
            functions = descriptor.Methods?.Select(m => new {m.Key, Value = new FunctionBinder(descriptor.Id, m.Value, v8Serializer, functionCallRegistry)})
                .ToDictionary(k => k.Key, v => v.Value);

            propertyDescriptors = descriptor.Properties.Select(p => p.Value).OfType<CefPropertyDescriptor>().ToList();
        }

        public CefV8Value BindToNew()
        {
            var obj = CefV8Value.CreateObject();

            functions?.Values.ToList().ForEach(m => m.Bind(obj));

            propertyDescriptors.ForEach(c =>
            {
                var value = v8Serializer.Deserialize(c.ListValue);
                obj.SetValue(c.Name, value, CefV8PropertyAttribute.ReadOnly);
                c.ListValue.Dispose();
            });
            propertyDescriptors.Clear();

            return obj;
        }
    }
}
