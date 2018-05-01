using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Execution.Model;
using DSerfozo.RpcBindings.Extensions;
using DSerfozo.RpcBindings.Logging;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Execution
{
    public class MethodExecutor<TMarshal> : IMethodExecutor<TMarshal>, IBinder<TMarshal>
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly IReadOnlyDictionary<long, ObjectDescriptor> objects;
        private readonly BindingDelegate<TMarshal> bindingDelegate;

        BindingDelegate<TMarshal> IBinder<TMarshal>.Binder => bindingDelegate;

        public MethodExecutor(IReadOnlyDictionary<long, ObjectDescriptor> objects, BindingDelegate<TMarshal> bindingDelegate)
        {
            this.objects = objects;
            this.bindingDelegate = bindingDelegate;
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
            var parameterBindings = parameters.Select((t, i) => new Binding<TMarshal>
                {
                    TargetType = t.Type,
                    Value = actualParameters[i]
                })
                .ToList();

            var boundParameters = parameterBindings.Select(this.BindToNet).ToList();

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
                        actualResult = executeResult.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(executeResult);
                    }
                }
                else
                {
                    actualResult = executeResult;
                }

                result.Result = this.BindToWire(actualResult, methodDescriptor.ReturnValueAttribute);

                result.Success = true;
            }
            catch(TargetInvocationException e)
            {
                Log.ErrorException("Execute", e);
                result.Error = e.InnerException?.Message;
            }
            catch(Exception e)
            {
                Log.ErrorException("Execute", e);
                result.Error = e.Message;
            }

            return result;
        }
    }
}
