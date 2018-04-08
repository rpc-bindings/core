using DSerfozo.RpcBindings.Contract;
using System.Threading;
using DSerfozo.RpcBindings.Contract.Analyze;

namespace DSerfozo.RpcBindings.Analyze
{
    public sealed class IntIdGenerator : IIdGenerator
    {
        private long lastId;

        public long GetNextId()
        {
            return Interlocked.Increment(ref lastId);
        }
    }
}
