using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Analyze;
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

            public ObjectDescriptor AddBinding(object obj, AnalyzeOptions analyzeOptions)
            {
                throw new InvalidOperationException("This instance is read-only.");
            }

            public ObjectDescriptor AddDisposableBinding(IDisposable obj, AnalyzeOptions analyzeOptions)
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

        public static ObjectDescriptor AddBinding(this IBindingRepository @this, string name, object obj)
        {
            return @this.AddBinding(obj, new AnalyzeOptions
            {
                Name = name,
                AnalyzeProperties = true
            });
        }

        public static ObjectDescriptor AddDisposableBinding(this IBindingRepository @this, string name, IDisposable obj)
        {
            return @this.AddDisposableBinding(obj, new AnalyzeOptions
            {
                Name = name,
                AnalyzeProperties = true
            });
        }
    }
}
