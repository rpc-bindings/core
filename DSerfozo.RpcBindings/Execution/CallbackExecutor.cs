using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Communication;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Calling
{
    public class CallbackExecutor<TMarshal> : ICallbackExecutor
    {
        private readonly IDictionary<int, TaskCompletionSource<object>> callbacksInProgress = new Dictionary<int, TaskCompletionSource<object>>();
        private readonly IConnection<TMarshal> connection;
        private readonly IParameterBinder<TMarshal> parameterBinder;
        private int lastInvocationId;

        public CallbackExecutor(IConnection<TMarshal> connection, IParameterBinder<TMarshal> parameterBinder)
        {
            this.connection = connection;
            this.parameterBinder = parameterBinder;

            connection.RpcResponse += Connection_RpcResponse;
        }

        public void DeleteCallback(int id)
        {
            connection.Send(new RpcRequest<TMarshal>()
            {
                DeleteCallback = new DeleteCallback
                {
                    Id = id
                }
            });
        }

        public Task<object> Execute(int id, object[] args)
        {
            var nextId = Interlocked.Increment(ref lastInvocationId);
            var tcs = new TaskCompletionSource<object>();

            callbacksInProgress.Add(nextId, tcs);

            connection.Send(new RpcRequest<TMarshal>()
            {
                CallbackExecution = new CallbackExecution<TMarshal>
                {
                    Id = id,
                    Parameters = args.Select(s => parameterBinder.BindToWire(s)).ToArray()
                }
            });

            return tcs.Task;
        }

        private void Connection_RpcResponse(RpcResponse<TMarshal> obj)
        {
            var callbackResult = obj.CallbackResult;
            if (callbackResult != null)
            {
                TaskCompletionSource<object> tcs;
                if(callbacksInProgress.TryGetValue(callbackResult.Id, out tcs))
                {
                    if(callbackResult.IsSuccess)
                    {
                        tcs.TrySetResult(parameterBinder.BindToNet(new ParameterBinding<TMarshal> { Value = callbackResult.Result }));
                    }
                    else
                    {
                        tcs.TrySetException(new Exception(callbackResult.Error));
                    }
                }
            }
        }
    }
}
