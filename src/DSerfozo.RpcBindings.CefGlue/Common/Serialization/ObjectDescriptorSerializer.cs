using System.Collections.Generic;
using System.Linq;
using DSerfozo.RpcBindings.Model;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Common.Serialization
{
    public static class ObjectDescriptorSerializer
    {
        public static void ToCefList(this ObjectDescriptor descriptor, CefListValue cefList)
        {
            cefList.SetInt(0, descriptor.Id);
            cefList.SetString(1, descriptor.Name);
            var methodsList = CefListValue.Create();
            var values = descriptor.Methods.Values.ToList();
            for (var i = 0; i < values.Count; i++)
            {
                var method = values[i];
                var methodValue = CefListValue.Create();
                method.ToCefList(methodValue);
                methodsList.SetList(i, methodValue);
            }
            cefList.SetList(2, methodsList);
        }

        public static void ToCefList(this MethodDescriptor descriptor, CefListValue cefList)
        {
            cefList.SetInt(0, descriptor.Id);
            cefList.SetString(1, descriptor.Name);
        }

        public static ObjectDescriptor ReadObjectDescriptor(CefListValue cefList)
        {
            var id = cefList.GetInt(0);
            var name = cefList.GetString(1);
            var methods = cefList.GetList(2);

            var methodDescriptors = new List<MethodDescriptor>(methods.Count);
            for (var i = 0; i < methods.Count; i++)
            {
                var method = methods.GetList(i);
                methodDescriptors.Add(ReadMethodDescriptor(method));
            }

            return ObjectDescriptor.Create().WithId(id).WithName(name).WithMethods(methodDescriptors).Get();
        }

        public static MethodDescriptor ReadMethodDescriptor(CefListValue cefList)
        {
            var id = cefList.GetInt(0);
            var name = cefList.GetString(1);

            return MethodDescriptor.Create().WithId(id).WithName(name).Get();
        }
    }
}
