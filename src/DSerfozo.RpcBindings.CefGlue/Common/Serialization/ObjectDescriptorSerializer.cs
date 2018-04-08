using System.Collections.Generic;
using System.Linq;
using DSerfozo.RpcBindings.CefGlue.Renderer.Model;
using DSerfozo.RpcBindings.CefGlue.Renderer.Serialization;
using DSerfozo.RpcBindings.Model;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Common.Serialization
{
    public static class ObjectDescriptorSerializer
    {
        public static void ToCefList(this ObjectDescriptor descriptor, CefDictionaryValue cefDictionary)
        {
            cefDictionary.SetInt64(nameof(ObjectDescriptor.Id), descriptor.Id);
            cefDictionary.SetString(nameof(ObjectDescriptor.Name), descriptor.Name);
            using (var methodsList = CefListValue.Create())
            using (var propertyList = CefListValue.Create())
            {
                var methods = descriptor.Methods.Values.ToList();
                for (var i = 0; i < methods.Count; i++)
                {
                    var method = methods[i];
                    using (var methodValue = CefDictionaryValue.Create())
                    {
                        methodValue.SetInt64(nameof(MethodDescriptor.Id), method.Id);
                        methodValue.SetString(nameof(MethodDescriptor.Name), method.Name);

                        methodsList.SetDictionary(i, methodValue);
                    }
                }

                var properties = descriptor.Properties?.Values.ToList() ?? new List<PropertyDescriptor>();
                for (var i = 0; i < properties.Count; i++)
                {
                    var property = properties[i];
                    using (var propertyValue = CefDictionaryValue.Create())
                    {
                        propertyValue.SetInt64(nameof(PropertyDescriptor.Id), property.Id);
                        propertyValue.SetString(nameof(PropertyDescriptor.Name), property.Name);
                        propertyValue.SetValue(nameof(PropertyDescriptor.Value),
                            ObjectSerializer.Serialize(property.Value, new HashSet<object>()));

                        propertyList.SetDictionary(i, propertyValue);
                    }
                }

                cefDictionary.SetList(nameof(ObjectDescriptor.Methods), methodsList);
                cefDictionary.SetList(nameof(ObjectDescriptor.Properties), propertyList);
            }
        }

        public static ObjectDescriptor ReadObjectDescriptor(CefDictionaryValue cefList, V8Serializer v8Serializer)
        {
            var id = cefList.GetInt64(nameof(ObjectDescriptor.Id));
            var name = cefList.GetString(nameof(ObjectDescriptor.Name));
            using (var methods = cefList.GetList(nameof(ObjectDescriptor.Methods)))
            using (var properties = cefList.GetList(nameof(ObjectDescriptor.Properties)))
            {
                var methodDescriptors = new List<MethodDescriptor>(methods.Count);
                for (var i = 0; i < methods.Count; i++)
                {
                    using (var method = methods.GetDictionary(i))
                    {
                        methodDescriptors.Add(MethodDescriptor.Create()
                            .WithId(method.GetInt64(nameof(MethodDescriptor.Id)))
                            .WithName(method.GetString(nameof(MethodDescriptor.Name)))
                            .Get());
                    }
                }


                var propertyDescriptors = new List<PropertyDescriptor>(properties.Count);
                for (var i = 0; i < properties.Count; i++)
                {
                    using (var property = properties.GetDictionary(i))
                    using(var val = property.GetValue(nameof(PropertyDescriptor.Value)))
                    {
                        propertyDescriptors.Add(new CefPropertyDescriptor(
                            property.GetInt64(nameof(PropertyDescriptor.Id)),
                            property.GetString(nameof(PropertyDescriptor.Name)),
                            val.Copy()));
                    }
                }

                return ObjectDescriptor.Create().WithId(id).WithName(name).WithMethods(methodDescriptors)
                    .WithProperties(propertyDescriptors).Get();
            }
        }
    }
}
