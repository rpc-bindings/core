using System;
using System.Linq;
using DSerfozo.RpcBindings.CefGlue.Common.Serialization;
using DSerfozo.RpcBindings.CefGlue.Renderer.Serialization;
using DSerfozo.RpcBindings.CefGlue.Renderer.Services;
using DSerfozo.RpcBindings.Contract.Communication.Model;
using DSerfozo.RpcBindings.Execution.Model;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Renderer.Util
{
    public class Callback : IDisposable
    {
        private readonly CefV8Value function;
        private readonly PromiseService promiseService;
        private readonly V8Serializer v8Serializer;
        private readonly CefV8Context context;

        public CefV8Context Context => context;

        public Callback(CefV8Value function,  PromiseService promiseService, V8Serializer v8Serializer)
        {
            this.function = function;
            this.promiseService = promiseService;
            this.v8Serializer = v8Serializer;

            context = CefV8Context.GetCurrentContext();
        }

        public void Execute(CallbackExecution<CefValue> execution)
        {
            CefV8Exception exception = null;
            var cefV8Values = execution.Parameters.Select(s => v8Serializer.Deserialize(s)).ToArray();
            var result = function.ExecuteFunctionWithContext(context, null,
                cefV8Values);
            var browser = context.GetBrowser();

            if (result == null && function.HasException)
            {
                exception = function.GetException();
                function.ClearException();
            }

            if (promiseService.IsPromise(result, context))
            {
                promiseService.Then(result, context, a => CallbackDone(a, browser, execution.ExecutionId));
            }
            else
            {
                CallbackDone(new PromiseResult
                {
                    Success = result != null,
                    Result = result,
                    Error = exception?.Message
                }, browser, execution.ExecutionId);
            }
            
        }

        private void CallbackDone(PromiseResult promiseResult, CefBrowser browser, int executionId)
        {
            var callbackResult = new CallbackResult<CefValue>
            {
                ExecutionId = executionId,
                Success = promiseResult.Success
            };

            if (promiseResult.Success)
            {
                callbackResult.Result = v8Serializer.Serialize(promiseResult.Result);
            }
            else if(!string.IsNullOrWhiteSpace(promiseResult.Error))
            {
                callbackResult.Error = promiseResult.Error;
            }

            browser.SendProcessMessage(CefProcessId.Browser, new RpcResponse<CefValue>
            {
                CallbackResult = callbackResult
            }.ToCefProcessMessage());
        }

        public void Dispose()
        {
            function.Dispose();
            context.Dispose();
        }
    }
}
