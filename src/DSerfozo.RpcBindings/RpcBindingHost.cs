using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Analyze;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Communication.Model;
using DSerfozo.RpcBindings.Execution;
using DSerfozo.RpcBindings.Execution.Model;
using DSerfozo.RpcBindings.Marshaling;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings
{
    public abstract class RpcBindingHost<TMarshal> : IDisposable
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IConnection<TMarshal> connection;
        private readonly IMethodExecutor<TMarshal> methodExecutor;
        private readonly IPropertyExecutor<TMarshal> propertyExecutor;
        private readonly IBindingRepository bindingRepository;
        private bool disposed;

        public event Func<ResolvingBoundObjectArgs, Task> ResolvingBoundObject;

        public IConnection<TMarshal> Connection => connection;

        public IBindingRepository Repository => bindingRepository;

        protected RpcBindingHost(IConnection<TMarshal> connection, Func<ICallbackFactory<TMarshal>, IParameterBinder<TMarshal>> parameterBinderFactory)
        {
            this.connection = connection;
            bindingRepository = new BindingRepository(new IntIdGenerator());

            var incomingMessages = Observable.FromEvent<Action<RpcResponse<TMarshal>>, RpcResponse<TMarshal>>(handler => connection.RpcResponse += handler, handler => connection.RpcResponse -= handler);

            var callbackExecutor = new CallbackExecutor<TMarshal>(new IntIdGenerator(), incomingMessages.Select(m => m.CallbackResult).Where(m => m != null));
            var callbackFactory = new CallbackFactory<TMarshal>(callbackExecutor);
            var parameterBinder = parameterBinderFactory(callbackFactory);
            methodExecutor = new MethodExecutor<TMarshal>(bindingRepository.Objects, parameterBinder);
            propertyExecutor = new PropertyExecutor<TMarshal>(bindingRepository.Objects, parameterBinder);

            disposables.Add(callbackExecutor.Subscribe<DeleteCallback>(OnDeleteCallback));
            disposables.Add(callbackExecutor.Subscribe<CallbackExecution<TMarshal>>(OnCallbackExecution));
            disposables.Add(incomingMessages.Select(m => m.MethodExecution).Where(m => m != null).Subscribe(OnMethodExecution));
            disposables.Add(incomingMessages.Select(m => m.PropertyGet).Where(m => m != null).Subscribe(OnPropertyGetExecution));
            disposables.Add(incomingMessages.Select(m => m.PropertySet).Where(m => m != null).Subscribe(OnPropertySetExecution));
            disposables.Add(incomingMessages.Select(m => m.DynamicObjectRequest).Where(m => m != null).Subscribe(OnDyanmicObjectRequest));
        }

        private async void OnDyanmicObjectRequest(DynamicObjectRequest dynamicObjectRequest)
        {
            var response = new DynamicObjectResponse
            {
                ExecutionId = dynamicObjectRequest.ExecutionId
            };

            if (!Repository.TryGetObjectByName(dynamicObjectRequest.Name, out var objectDescriptor))
            {
                objectDescriptor = await ResolveDynamicObject(dynamicObjectRequest, response, objectDescriptor);
            }
            else
            {
                response.Success = true;
            }

            response.ObjectDescriptor = objectDescriptor;
            await connection.Send(new RpcRequest<TMarshal>
            {
                DynamicObjectResult = response
            });
        }

        private async Task<ObjectDescriptor> ResolveDynamicObject(DynamicObjectRequest dynamicObjectRequest, DynamicObjectResponse response, ObjectDescriptor objectDescriptor)
        {
            var handler = ResolvingBoundObject;
            var invocationList = handler?.GetInvocationList();
            if (invocationList != null)
            {
                var args = new ResolvingBoundObjectArgs(dynamicObjectRequest.Name);
                var tasks = invocationList.OfType<Func<ResolvingBoundObjectArgs, Task>>()
                    .Select(i => i(args)).ToArray();
                try
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);

                    if (args.Object == null)
                    {
                        throw new InvalidOperationException("Could not resolve dynamic object.");
                    }

                    if (args.Disposable && args.Object is IDisposable)
                    {
                        objectDescriptor = Repository.AddDisposableBinding(dynamicObjectRequest.Name, (IDisposable)args.Object);
                    }
                    else
                    {
                        objectDescriptor = Repository.AddBinding(dynamicObjectRequest.Name, args.Object);
                    }

                    response.Success = true;
                }
                catch (AggregateException e)
                {
                    var flattened = e.Flatten();
                    var exception = flattened.InnerException;
                    response.Exception = exception?.Message;
                }
                catch (Exception e)
                {
                    response.Exception = e.Message;
                }
            }

            return objectDescriptor;
        }

        private void OnPropertyGetExecution(PropertyGetExecution propertyGetExecution)
        {
            var result = propertyExecutor.Execute(propertyGetExecution);

            connection.Send(new RpcRequest<TMarshal>() { PropertyResult = result });
        }

        private void OnPropertySetExecution(PropertySetExecution<TMarshal> propertySetExecution)
        {
            var result = propertyExecutor.Execute(propertySetExecution);

            connection.Send(new RpcRequest<TMarshal>() { PropertyResult = result });
        }

        private async void OnCallbackExecution(CallbackExecution<TMarshal> callbackExecution)
        {
            await connection.Send(new RpcRequest<TMarshal>() { CallbackExecution = callbackExecution }).ConfigureAwait(false);
        }

        private async void OnDeleteCallback(DeleteCallback deleteCallback)
        {
            await connection.Send(new RpcRequest<TMarshal>() { DeleteCallback = deleteCallback }).ConfigureAwait(false);
        }

        private async void OnMethodExecution(MethodExecution<TMarshal> methodExecution)
        {
            var result = await methodExecutor.Execute(methodExecution);

            await connection.Send(new RpcRequest<TMarshal>() { MethodResult = result }).ConfigureAwait(false);
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
