using System;
using DSerfozo.RpcBindings.Analyze;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DSerfozo.RpcBindings.Contract.Analyze;

namespace DSerfozo.RpcBindings
{
    public class BindingRepository : IBindingRepository
    {
        private readonly ISet<IDisposable> disposables = new HashSet<IDisposable>();
        private readonly IDictionary<long, ObjectDescriptor> objects = new Dictionary<long, ObjectDescriptor>();
        private readonly ObjectAnalyzer objectAnalyzer;
        private bool disposed;

        public IReadOnlyDictionary<long, ObjectDescriptor> Objects => new ReadOnlyDictionary<long, ObjectDescriptor>(objects);

        public BindingRepository(IIdGenerator idGenerator)
        {
            objectAnalyzer = new ObjectAnalyzer(idGenerator, 
                new PropertyAnalyzer(idGenerator, new CamelCaseNameGenerator()), 
                new MethodAnalyzer(idGenerator, new CamelCaseNameGenerator()));
        }

        public ObjectDescriptor AddBinding(object obj, AnalyzeOptions options)
        {
            ThrowIfDisposed();

            var objectDescriptor = objectAnalyzer.AnalyzeObject(obj, options);
            objects.Add(objectDescriptor.Id, objectDescriptor);

            return objectDescriptor;
        }

        public ObjectDescriptor AddDisposableBinding(IDisposable obj, AnalyzeOptions options)
        {
            ThrowIfDisposed();

            var objectDescriptor = objectAnalyzer.AnalyzeObject(obj, options);
            disposables.Add(obj);
            objects.Add(objectDescriptor.Id, objectDescriptor);

            return objectDescriptor;
        }

        public bool TryGetObjectByName(string name, out ObjectDescriptor objectDescriptor)
        {
            ThrowIfDisposed();

            objectDescriptor = objects.Values.FirstOrDefault(o => o.Name == name);
            return objectDescriptor != null;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposables.ToList().ForEach(d => d.Dispose());
                disposables.Clear();
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
