using System.Collections.Generic;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IBindingRepository
    {
        IReadOnlyDictionary<int, ObjectDescriptor> Objects { get; }

        void AddBinding<TObject>(string key, TObject obj);

        void AddBinding(string key, object obj);
    }
}