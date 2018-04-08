using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Contract.Analyze;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IBindingRepository : IDisposable
    {
        IReadOnlyDictionary<long, ObjectDescriptor> Objects { get; }

        ObjectDescriptor AddBinding(object obj, AnalyzeOptions analyzeOptions);

        ObjectDescriptor AddDisposableBinding(IDisposable obj, AnalyzeOptions analyzeOptions);

        bool TryGetObjectByName(string name, out ObjectDescriptor objectDescriptor);
    }
}