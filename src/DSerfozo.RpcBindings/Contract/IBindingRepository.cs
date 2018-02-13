using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IBindingRepository : IDisposable
    {
        IReadOnlyDictionary<int, ObjectDescriptor> Objects { get; }

        ObjectDescriptor AddBinding<TObject>(string key, TObject obj);

        ObjectDescriptor AddBinding(string key, object obj);

        ObjectDescriptor AddDisposableBinding(string key, IDisposable obj);

        bool TryGetObjectByName(string name, out ObjectDescriptor objectDescriptor);
    }
}