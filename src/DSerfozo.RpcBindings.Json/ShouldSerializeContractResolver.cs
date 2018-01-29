using System.Linq;
using System.Reflection;
using DSerfozo.RpcBindings.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DSerfozo.RpcBindings.Json
{
    public class ShouldSerializeContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var shouldSerialize = member.CustomAttributes.Any(s => s.AttributeType == typeof(ShouldSerializeAttribute) || s.AttributeType == typeof(JsonPropertyAttribute));
            if (!shouldSerialize)
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}
