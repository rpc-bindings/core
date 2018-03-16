using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DSerfozo.RpcBindings.Analyze
{
    public class PropertyAnalyzer : IPropertyAnalyzer
    {
        private readonly IIdGenerator idGenerator;
        private readonly IPropertyNameGenerator propertyNameGenerator;

        public PropertyAnalyzer(IIdGenerator idGenerator, IPropertyNameGenerator propertyNameGenerator)
        {
            this.idGenerator = idGenerator;
            this.propertyNameGenerator = propertyNameGenerator;
        }

        public IEnumerable<PropertyDescriptor> AnalyzeProperties(Type type)
        {
            var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !p.IsSpecialName && 
                            !p.IsDefined(typeof(BindingIgnoreAttribute)) &&
                            p.GetIndexParameters().Length <= 0);
            foreach(var propertyInfo in propertyInfos)
            {
                yield return PropertyDescriptor.Create()
                    .WithId(idGenerator.GetNextId())
                    .WithName(propertyNameGenerator.GetBoundPropertyName(propertyInfo.Name))
                    .WithReadOnly(IsReadOnly(propertyInfo))
                    .WithGetter(o => propertyInfo.GetValue(o))
                    .WithSetter((o, v) => propertyInfo.SetValue(o, v))
                    .WithType(propertyInfo.PropertyType)
                    .Get();
            }
        }

        private static bool IsReadOnly(PropertyInfo propertyInfo)
        {
            return !propertyInfo.CanWrite || propertyInfo.SetMethod?.Attributes.HasFlag(MethodAttributes.Private) == true;
        }
    }
}
