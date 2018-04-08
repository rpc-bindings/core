using System;

namespace DSerfozo.RpcBindings.Contract.Marshaling
{
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    public class BindValueAttribute : Attribute
    {
        public bool ExtractPropertyValues { get; set; }
    }
}
