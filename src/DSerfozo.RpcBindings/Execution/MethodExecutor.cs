using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Execution.Model;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Execution
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

        public async Task<MethodResult<TMarshal>> Execute(MethodExecution<TMarshal> methodExcecution)
        {
            if(!objects.TryGetValue(methodExcecution.ObjectId, out var objectDescriptor))
            {
                throw new InvalidOperationException("");
            }

            if(!objectDescriptor.Methods.TryGetValue(methodExcecution.MethodId, out var methodDescriptor))
            {
                throw new InvalidOperationException("");
            }

            var result = new MethodResult<TMarshal>
            {
                ExecutionId = methodExcecution.ExecutionId
            };
            if(methodDescriptor.ParameterCount > 0 && methodDescriptor.ParameterCount != methodExcecution.Parameters.Length)
            {
                result.Error = "Parameter mismatch.";
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
                    result.Error = "Parameter mismatch.";
                    return result;
                }
            }

            try
            {
                var executeResult = methodDescriptor.Execute(objectDescriptor.Object, boundParameters.ToArray());

                object actualResult = null;
                if (executeResult is Task)
                {
                    await (executeResult as Task).ConfigureAwait(false);

                    if (executeResult.GetType().IsGenericType)
                    {
                        actualResult = executeResult.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(executeResult);
                    }
                }
                else
                {
                    actualResult = executeResult;
                }

                result.Result = parameterBinder.BindToWire(actualResult);

                result.Success = true;
            }
            catch(TargetInvocationException e)
            {
                result.Error = e.InnerException?.Message;
            }
            catch(Exception e)
            {
                result.Error = e.Message;
            }

            return result;
        }
    }
}
