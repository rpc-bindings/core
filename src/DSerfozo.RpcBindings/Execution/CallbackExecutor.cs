using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Analyze;
using DSerfozo.RpcBindings.Contract.Communication;
using DSerfozo.RpcBindings.Contract.Execution;
using DSerfozo.RpcBindings.Contract.Execution.Model;
using DSerfozo.RpcBindings.Contract.Marshaling;
using DSerfozo.RpcBindings.Execution.Model;
using DSerfozo.RpcBindings.Extensions;

namespace DSerfozo.RpcBindings.Execution
{
    public class CallbackExecutor<TMarshal> : ICallbackExecutor<TMarshal>
    {
        private class PendingExecution : IBinder<TMarshal>
        {
            public TaskCompletionSource<object> Tcs { get; }

            public Type TargetType { get; set; }

            public BindingDelegate<TMarshal> Binder { get; internal set; }

            public PendingExecution()
            {
                Tcs = new TaskCompletionSource<object>();
            }

        }

        private readonly IDictionary<long, PendingExecution> callbacksInProgress = new Dictionary<long, PendingExecution>();
        private readonly ISubject<CallbackExecution<TMarshal>> callbackExecutionSubject = new Subject<CallbackExecution<TMarshal>>();
        private readonly ISubject<DeleteCallback> deleteSubject = new Subject<DeleteCallback>();
        private readonly IIdGenerator idGenerator;
        private readonly IConnectionAvailability connectionAvailable;

        public bool CanExecute => connectionAvailable.IsOpen;

        public CallbackExecutor(IIdGenerator idGenerator, IConnectionAvailability connectionAvailable, IObservable<CallbackResult<TMarshal>> resultStream)
        {
            resultStream.Subscribe(OnCallbackResult);
            this.idGenerator = idGenerator;
            this.connectionAvailable = connectionAvailable;
        }

        public IDisposable Subscribe(IObserver<CallbackExecution<TMarshal>> observer)
        {
            return callbackExecutionSubject.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<DeleteCallback> observer)
        {
            return deleteSubject.Subscribe(observer);
        }

        public Task<object> Execute(CallbackExecutionParameters<TMarshal> execute)
        {
            if (!CanExecute)
            {
                throw new InvalidOperationException("Cannot execute.");
            }

            var pending = new PendingExecution
            {
                TargetType = execute.ResultTargetType,
                Binder = execute.Binder
            };
            var nextId = idGenerator.GetNextId();
            callbacksInProgress.Add(nextId, pending);

            callbackExecutionSubject.OnNext(new CallbackExecution<TMarshal>
            {
                ExecutionId = nextId,
                FunctionId = execute.Id,
                Parameters = execute.Parameters.Select(s => execute.BindToWire(s.Value, s.Bindable)).ToArray(),
            });

            return pending.Tcs.Task;
        }

        public void DeleteCallback(long id)
        {
            if (!CanExecute)
            {
                throw new InvalidOperationException("Cannot delete.");
            }

            deleteSubject.OnNext(new DeleteCallback
            {
                FunctionId = id
            });
        }

        private void OnCallbackResult(CallbackResult<TMarshal> callbackResult)
        {
            if (callbacksInProgress.TryGetValue(callbackResult.ExecutionId, out var pending))
            {
                if (callbackResult.Success)
                {
                    pending.Tcs.TrySetResult(pending.BindToNet(new Binding<TMarshal> { Value = callbackResult.Result, TargetType = pending.TargetType }));
                }
                else
                {
                    pending.Tcs.TrySetException(new Exception(callbackResult.Error));
                }
            }
        }
    }
}
