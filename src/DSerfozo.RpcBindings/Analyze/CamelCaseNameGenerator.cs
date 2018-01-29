using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Analyze
{
    public class CamelCaseNameGenerator : IPropertyNameGenerator, IMethodNameGenerator
    {
        public string GetBoundMethodName(string methodName)
        {
            return CamelCaseName(methodName);
        }

        public string GetBoundPropertyName(string propertyName)
        {
            return CamelCaseName(propertyName);
        }

        private static string CamelCaseName(string methodName)
        {
            return methodName.Substring(0, 1).ToLower() + methodName.Substring(1);
        }
    }
}
