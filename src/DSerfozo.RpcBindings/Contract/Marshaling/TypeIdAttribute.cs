using System;

namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TypeIdAttribute : Attribute
    {
        public string Id { get; }

        public TypeIdAttribute(string id)
        {
            Id = id;
        }
    }
}
