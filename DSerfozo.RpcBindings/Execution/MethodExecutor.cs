using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Calling
{
    public class MethodExecutor<TMarshal> : IMethodExecutor<TMarshal>
    {
        private readonly IReadOnlyDictionary<int, ObjectDescriptor> objects;
        private readonly IParameterBinder<TMarshal> parameterBinder;

        public MethodExecutor(IReadOnlyDictionary<int, ObjectDescriptor> objects, IParameterBinder<TMarshal> parameterBinder)
        {
            this.objects = objects;
            this.parameterBinder = parameterBinder;
        }

        public async Task<MethodResult> Execute(MethodExecution<TMarshal> methodExcecution)
        {
            ObjectDescriptor objectDescriptor;
            if(!objects.TryGetValue(methodExcecution.ObjectId, out objectDescriptor))
            {
                throw new InvalidOperationException("");
            }

            MethodDescriptor methodDescriptor;
            if(!objectDescriptor.Methods.TryGetValue(methodExcecution.MethodId, out methodDescriptor))
            {
                throw new InvalidOperationException("");
            }

            var result = new MethodResult()
            {
                Key = methodExcecution.Key
            };
            if(methodDescriptor.ParameterCount > 0 && methodDescriptor.ParameterCount != methodExcecution.Parameters.Length)
            {
                result.Error = new ParameterMismatchException();
                return result;
            }

            var parameters = methodDescriptor.Parameters.ToArray();

            var actualParameters = methodExcecution.Parameters.ToList();
            List<ParameterBinding<TMarshal>> parameterBindings = new List<ParameterBinding<TMarshal>>();
            for (var i = 0; i < parameters.Length; i++)
            {
                parameterBindings.Add(new ParameterBinding<TMarshal>
                {
                    TargetType = parameters[i].Type,
                    Value = (TMarshal)actualParameters[i]
                });
            }

            var boundParameters = parameterBindings.Select(p => parameterBinder.BindToNet(p)).ToList();

            var parameterTypes = boundParameters.Count > 0 ? boundParameters.Select(p => p?.GetType()).ToArray() : new Type[] { };
            for (var i = 0; i < parameterTypes.Length; i++)
            {
                if ((parameterTypes[i] != null || parameters[i].Type.IsValueType) && !parameters[i].Type.IsAssignableFrom(parameterTypes[i]))
                {
                    result.Error = new ParameterMismatchException();
                    return result;
                }
            }

            try
            {
                var executeResult = methodDescriptor.Execute(objectDescriptor.Object, boundParameters.ToArray());

                if (executeResult is Task)
                {
                    await (executeResult as Task).ConfigureAwait(false);

                    if (executeResult.GetType().IsGenericType)
                    {
                        result.Result = executeResult.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(executeResult);
                    }
                }
                else
                {
                    result.Result = executeResult;
                }

                result.IsSuccess = true;
            }
            catch(TargetInvocationException e)
            {
                result.Error = e.InnerException;
            }
            catch(Exception e)
            {
                result.Error = e;
            }

            return result;
        }
    }
}
