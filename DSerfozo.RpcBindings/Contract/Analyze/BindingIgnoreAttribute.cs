using System;

namespace DSerfozo.RpcBindings.Contract
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class BindingIgnoreAttribute : Attribute
    {
    }
}
