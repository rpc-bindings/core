using System;
using DSerfozo.RpcBindings.Analyze;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DSerfozo.RpcBindings
{
    public class BindingRepository : IBindingRepository
    {
        private readonly IDictionary<int, ObjectDescriptor> objects = new Dictionary<int, ObjectDescriptor>();
        private readonly ObjectAnalyzer objectAnalyzer;
        private bool disposed;

        public IReadOnlyDictionary<int, ObjectDescriptor> Objects => new ReadOnlyDictionary<int, ObjectDescriptor>(objects);

        public BindingRepository(IIdGenerator idGenerator)
        {
            objectAnalyzer = new ObjectAnalyzer(idGenerator, 
                new PropertyAnalyzer(idGenerator, new CamelCaseNameGenerator()), 
                new MethodAnalyzer(idGenerator, new CamelCaseNameGenerator()));
        }

        public void AddBinding<TObject>(string key, TObject obj)
        {
            ThrowIfDisposed();

            var objectDescriptor = objectAnalyzer.AnalyzeObject(key, obj);
            objects.Add(objectDescriptor.Id, objectDescriptor);
        }

        public void AddBinding(string key, object obj)
        {
            ThrowIfDisposed();

            var objectDescriptor = objectAnalyzer.AnalyzeObject(key, obj);
            objects.Add(objectDescriptor.Id, objectDescriptor);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                objects.Values.Select(s => s.Object).OfType<IDisposable>().ToList().ForEach(o => o.Dispose());
                objects.Clear();

                disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(BindingRepository));
            }
        }
    }
}
