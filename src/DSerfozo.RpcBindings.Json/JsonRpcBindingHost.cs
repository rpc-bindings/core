using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DSerfozo.RpcBindings.Analyze;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Communication.Model;
using DSerfozo.RpcBindings.Execution;
using DSerfozo.RpcBindings.Execution.Model;
using DSerfozo.RpcBindings.Marshaling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSerfozo.RpcBindings.Json
{
    public class JsonRpcBindingHost : IDisposable
    {
        private readonly JsonSerializer jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new ShouldSerializeContractResolver()
        };
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IConnection<JToken> connection;
        private readonly IBindingRepository bindingRepository;
        private readonly IMethodExecutor<JToken> methodExecutor;
        private bool disposed;

        public IConnection<JToken> Connection => connection;

        public IBindingRepository Repository => bindingRepository;

        public JsonRpcBindingHost(Func<JsonSerializer, IConnection<JToken>> connectionFactory)
        {
            connection = connectionFactory(jsonSerializer);

            var incomingMessages = Observable.FromEvent<Action<RpcResponse<JToken>>, RpcResponse<JToken>>(handler => connection.RpcResponse += handler, handler => connection.RpcResponse -= handler); 

            bindingRepository = new BindingRepository(new IntIdGenerator());
            ICallbackExecutor<JToken> callbackExecutor = new CallbackExecutor<JToken>(new IntIdGenerator(), incomingMessages.Select(m => m.CallbackResult).Where(m => m != null));
            ICallbackFactory<JToken> callbackFactory = new CallbackFactory<JToken>(callbackExecutor);
            IParameterBinder<JToken> parameterBinder = new JsonBinder(jsonSerializer, callbackFactory);
            methodExecutor = new MethodExecutor<JToken>(bindingRepository.Objects, parameterBinder);

            disposables.Add(callbackExecutor.Subscribe<DeleteCallback>(OnDeleteCallback));
            disposables.Add(callbackExecutor.Subscribe<CallbackExecution<JToken>>(OnCallbackExecution));
            disposables.Add(incomingMessages.Select(m => m.MethodExecution).Where(m => m != null).Subscribe(OnMethodExecution));
        }

        private async void OnCallbackExecution(CallbackExecution<JToken> callbackExecution)
        {
            await connection.Send(new RpcRequest<JToken>() { CallbackExecution = callbackExecution }).ConfigureAwait(false);
        }

        private async void OnDeleteCallback(DeleteCallback deleteCallback)
        {
            await connection.Send(new RpcRequest<JToken>() { DeleteCallback = deleteCallback }).ConfigureAwait(false);
        }

        private async void OnMethodExecution(MethodExecution<JToken> methodExecution)
        {
            var result = await methodExecutor.Execute(methodExecution);

            await connection.Send(new RpcRequest<JToken>() { MethodResult = result }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposables.Dispose();
                bindingRepository.Dispose();

                disposed = true;
            }
        }
    }
}
