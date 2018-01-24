using DSerfozo.RpcBindings.Contract;
using System.Threading;

namespace DSerfozo.RpcBindings.Analyze
{
    public sealed class IntIdGenerator : IIdGenerator
    {
        private int lastId;

        public int GetNextId()
        {
            return Interlocked.Increment(ref lastId);
        }
    }
}
