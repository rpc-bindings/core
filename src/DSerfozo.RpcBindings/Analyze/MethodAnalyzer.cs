using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DSerfozo.RpcBindings.Analyze
{
    public class MethodAnalyzer : IMethodAnalyzer
    {
        private readonly IIdGenerator idGenerator;
        private readonly IMethodNameGenerator methodNameGenerator;

        public MethodAnalyzer(IIdGenerator idGenerator, IMethodNameGenerator methodNameGenerator)
        {
            this.idGenerator = idGenerator;
            this.methodNameGenerator = methodNameGenerator;
        }

        public IEnumerable<MethodDescriptor> AnalyzeMethods(Type type)
        {
            var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => !m.IsSpecialName &&
                            m.DeclaringType != typeof(object) &&
                            !m.IsDefined(typeof(BindingIgnoreAttribute)));
            foreach (var methodInfo in methodInfos)
            {
                var parameterInfo = methodInfo.GetParameters();
                yield return MethodDescriptor.Create()
                    .WithId(idGenerator.GetNextId())
                    .WithName(methodNameGenerator.GetBoundMethodName(methodInfo.Name))
                    .WithResultType(methodInfo.ReturnType)
                    .WithParameterCount(parameterInfo.Length)
                    .WithParameters(parameterInfo.Select(pi => new MethodParameterDescriptor(pi.ParameterType, false)))
                    .WithExecute((o, a) => methodInfo.Invoke(o, a))
                    .Get();
            }
        }
    }
}
