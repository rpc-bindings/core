using System;

namespace DSerfozo.RpcBindings.Contract.Analyze
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class BindingIgnoreAttribute : Attribute
    {
    }
}
