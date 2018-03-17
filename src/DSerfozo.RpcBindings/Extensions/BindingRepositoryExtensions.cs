using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Extensions
{
    public static class BindingRepositoryExtensions
    {
        private class ReadOnlyBindingRepositry : IBindingRepository
        {
            private readonly IBindingRepository wrapped;

            public IReadOnlyDictionary<long, ObjectDescriptor> Objects => wrapped.Objects;

            public ReadOnlyBindingRepositry(IBindingRepository wrapped)
            {
                this.wrapped = wrapped;
            }

            public ObjectDescriptor AddBinding<TObject>(string key, TObject obj)
            {
                throw new InvalidOperationException("This instance is read-only.");
            }

            public ObjectDescriptor AddBinding(string key, object obj)
            {
                throw new InvalidOperationException("This instance is read-only.");
            }

            public ObjectDescriptor AddDisposableBinding(string key, IDisposable obj)
            {
                throw new InvalidOperationException("This instance is read-only.");
            }

            public bool TryGetObjectByName(string name, out ObjectDescriptor objectDescriptor)
            {
                return wrapped.TryGetObjectByName(name, out objectDescriptor);
            }

            public void Dispose()
            {
                throw new InvalidOperationException("Readonly wrapper is not disposable");
            }
        }

        public static IBindingRepository AsReadOnly(this IBindingRepository @this)
        {
            return new ReadOnlyBindingRepositry(@this);
        }
    }
}
