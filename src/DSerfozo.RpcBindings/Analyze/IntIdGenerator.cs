using DSerfozo.RpcBindings.Contract;
using System.Threading;

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
