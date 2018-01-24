using DSerfozo.RpcBindings.Analyze;
using DSerfozo.RpcBindings.Calling;
using DSerfozo.RpcBindings.Communication;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Model;
using Newtonsoft.Json.Linq;

namespace DSerfozo.RpcBindings
{
    public class RpcBindingHost
    {
        private readonly IConnection<JToken> connection;
        private readonly IBindingRepository bindingRepository;
        private readonly IParameterBinder<JToken> parameterBinder;
        private readonly IMethodExecutor<JToken> methodExecutor;
        private readonly ICallbackExecutor callbackExecutor;
        private readonly CallbackFactory callbackFactory = new CallbackFactory();

        public IBindingRepository Repository => bindingRepository;

        public RpcBindingHost(IConnection<JToken> connection)
        {
            this.connection = connection;
            
            this.bindingRepository = new BindingRepository(new IntIdGenerator());
            this.parameterBinder = new JsonBinder(callbackFactory);
            this.methodExecutor = new MethodExecutor<JToken>(bindingRepository.Objects, parameterBinder);
            this.callbackExecutor = new CallbackExecutor<JToken>(connection, parameterBinder);
            this.callbackFactory.CallbackExecutor = callbackExecutor;

            this.connection.RpcResponse += OnMethodExecution;
        }

        private async void OnMethodExecution(RpcResponse<JToken> rpcMessage)
        {
            if(rpcMessage.MethodExecution != null)
            {
                var result = await methodExecutor.Execute(rpcMessage.MethodExecution);

                await connection.Send(new RpcRequest<JToken>() { MethodResult = result }).ConfigureAwait(false);
            }
        }
    }
}
