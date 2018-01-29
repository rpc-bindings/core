using System;

namespace DSerfozo.RpcBindings.Model
{
    public sealed class MethodParameterDescriptor
    {
        public Type Type { get; }

        public bool ParamArray { get; }

        public MethodParameterDescriptor(Type type, bool paramArray)
        {
            Type = type;
            ParamArray = paramArray;
        }
    }
}
