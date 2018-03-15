using System;
using System.Linq;
using System.Reactive.Concurrency;
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
        private readonly CallbackExecutor<TMarshal> callbackExecutor;
        private readonly IParameterBinder<TMarshal> parameterBinder;
        private bool disposed;

        public event EventHandler<ResolvingBoundObjectArgs> ResolvingBoundObject;

        public IConnection<TMarshal> Connection => connection;

        public IBindingRepository Repository => bindingRepository;

        public ICallbackExecutor<TMarshal> CallbackExecutor => callbackExecutor;

        public IParameterBinder<TMarshal> ParameterBinder => parameterBinder;

        protected RpcBindingHost(IConnection<TMarshal> connection, Func<ICallbackFactory<TMarshal>, IParameterBinder<TMarshal>> parameterBinderFactory, IScheduler baseScheduler)
        {
            this.connection = connection;
            bindingRepository = new BindingRepository(new IntIdGenerator());

            if (baseScheduler is IDisposable)
            {
                disposables.Add(baseScheduler as IDisposable);
            }

            // ReSharper disable once InvokeAsExtensionMethod
            var baseMessages = Observable.ObserveOn(connection, baseScheduler);
            callbackExecutor = new CallbackExecutor<TMarshal>(new IntIdGenerator(),
                () => connection.IsOpen,
                baseMessages.Select(m => m.CallbackResult)
                    .Where(m => m != null));
            var callbackFactory = new CallbackFactory<TMarshal>(callbackExecutor);
            parameterBinder = parameterBinderFactory(callbackFactory);
            methodExecutor = new MethodExecutor<TMarshal>(bindingRepository.Objects, parameterBinder);
            propertyExecutor = new PropertyExecutor<TMarshal>(bindingRepository.Objects, parameterBinder);

            disposables.Add(Observable.ObserveOn((callbackExecutor as IObservable<DeleteCallback>), baseScheduler)
                .Subscribe(OnDeleteCallback));
            disposables.Add(Observable.ObserveOn((callbackExecutor as IObservable<CallbackExecution<TMarshal>>), baseScheduler)
                .Subscribe(OnCallbackExecution));

            // ReSharper disable once InvokeAsExtensionMethod
            
            disposables.Add(baseMessages
                .Select(m => m.MethodExecution)
                .Where(m => m != null)
                .Subscribe(OnMethodExecution));
            disposables.Add(baseMessages
                .Select(m => m.PropertyGet)
                .Where(m => m != null)
                .Subscribe(OnPropertyGetExecution));
            disposables.Add(baseMessages
                .Select(m => m.PropertySet)
                .Where(m => m != null)
                .Subscribe(OnPropertySetExecution));
            disposables.Add(baseMessages
                .Select(m => m.DynamicObjectRequest)
                .Where(m => m != null)
                .Subscribe(OnDyanmicObjectRequest));
        }

        private void OnDyanmicObjectRequest(DynamicObjectRequest dynamicObjectRequest)
        {
            var response = new DynamicObjectResponse
            {
                ExecutionId = dynamicObjectRequest.ExecutionId
            };

            if (!Repository.TryGetObjectByName(dynamicObjectRequest.Name, out var objectDescriptor))
            {
                objectDescriptor = ResolveDynamicObject(dynamicObjectRequest, response, objectDescriptor);
            }
            else
            {
                response.Success = true;
            }

            response.ObjectDescriptor = objectDescriptor;
            connection.Send(new RpcRequest<TMarshal>
            {
                DynamicObjectResult = response
            });
        }

        private ObjectDescriptor ResolveDynamicObject(DynamicObjectRequest dynamicObjectRequest,
            DynamicObjectResponse response, ObjectDescriptor objectDescriptor)
        {
            var args = new ResolvingBoundObjectArgs(dynamicObjectRequest.Name);
            try
            {
                ResolvingBoundObject?.Invoke(this, args);

                if (args.Object == null)
                {
                    return null;
                }

                if (args.Disposable && args.Object is IDisposable)
                {
                    objectDescriptor =
                        Repository.AddDisposableBinding(dynamicObjectRequest.Name, (IDisposable) args.Object);
                }
                else
                {
                    objectDescriptor = Repository.AddBinding(dynamicObjectRequest.Name, args.Object);
                }

                response.Success = true;
            }
            catch (Exception e)
            {
                response.Exception = e.Message;
            }

            return objectDescriptor;
        }

        private void OnPropertyGetExecution(PropertyGetExecution propertyGetExecution)
        {
            var result = propertyExecutor.Execute(propertyGetExecution);

            connection.Send(new RpcRequest<TMarshal>() {PropertyResult = result});
        }

        private void OnPropertySetExecution(PropertySetExecution<TMarshal> propertySetExecution)
        {
            var result = propertyExecutor.Execute(propertySetExecution);

            connection.Send(new RpcRequest<TMarshal>() {PropertyResult = result});
        }

        private void OnCallbackExecution(CallbackExecution<TMarshal> callbackExecution)
        {
            connection.Send(new RpcRequest<TMarshal>() {CallbackExecution = callbackExecution});
        }

        private void OnDeleteCallback(DeleteCallback deleteCallback)
        {
            connection.Send(new RpcRequest<TMarshal>() {DeleteCallback = deleteCallback});
        }

        private async void OnMethodExecution(MethodExecution<TMarshal> methodExecution)
        {
            var resultTask = methodExecutor.Execute(methodExecution);
            MethodResult<TMarshal> methodResult;
            if (resultTask.IsCompleted)
            {
                methodResult = resultTask.Result;
            }
            else
            {
                methodResult = await resultTask.ConfigureAwait(false);
            }

            connection.Send(new RpcRequest<TMarshal>() {MethodResult = methodResult });
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
