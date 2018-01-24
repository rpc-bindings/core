using DSerfozo.RpcBindings.Contract;
using System;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Marshaling
{
    public class Callback : ICallback
    {
        private readonly ICallbackExecutor executor;
        private readonly int id;
        private bool disposed;

        public bool CanExecute => !disposed;

        public Callback(int id, ICallbackExecutor executor)
        {
            this.id = id;
            this.executor = executor;
        }

        public void Dispose()
        {
            if(!disposed)
            {
                executor.DeleteCallback(id);
                disposed = true;
            }
        }

        public async Task<object> Execute(object[] args)
        {
            if(!CanExecute)
            {
                throw new ObjectDisposedException(nameof(Callback));
            }

            return await executor.Execute(id, args).ConfigureAwait(false);
        }
    }
}
