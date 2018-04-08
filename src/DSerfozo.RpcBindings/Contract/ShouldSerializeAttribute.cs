using System;

namespace DSerfozo.RpcBindings.Contract
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class ShouldSerializeAttribute : Attribute
    {
    }
}
