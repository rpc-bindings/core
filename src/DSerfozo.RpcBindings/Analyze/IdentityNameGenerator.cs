using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Analyze;

namespace DSerfozo.RpcBindings.Analyze
{
    public sealed class IdentityNameGenerator : IPropertyNameGenerator, IMethodNameGenerator
    {
        public string GetBoundMethodName(string methodName)
        {
            return methodName;
        }

        public string GetBoundPropertyName(string propertyName)
        {
            return propertyName;
        }
    }
}
