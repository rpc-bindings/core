using System;
using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Execution.Model
{
    public sealed class MethodResult
    {
        [ShouldSerialize]
        public int ExecutionId { get; set; }

        [ShouldSerialize]
        public bool Success { get; set; }

        [ShouldSerialize]
        public object Result { get; set; }

        [ShouldSerialize]
        public Exception Error { get; set; }
    }
}
