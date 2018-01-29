using System;

namespace DSerfozo.RpcBindings.Contract
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ShouldSerializeAttribute : Attribute
    {
    }
}
