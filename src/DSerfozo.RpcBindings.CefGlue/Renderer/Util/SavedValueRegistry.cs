using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace DSerfozo.RpcBindings.CefGlue.Renderer.Util
{
    public class SavedValueRegistry<TValue> : IDisposable where TValue : IDisposable
    {
        private readonly IDictionary<int, Tuple<long, TValue>> savedValues = new Dictionary<int, Tuple<long, TValue>>();
        private int lastId;

        public bool Has(int id)
        {
            return savedValues.ContainsKey(id);
        }

        public TValue Get(int id)
        {
            var result = default(TValue);
            if (savedValues.TryGetValue(id, out var valueTuple))
            {
                savedValues.Remove(id);
                result = valueTuple.Item2;
            }

            return result;
        }

        public int Save(long frameId, TValue value)
        {
            var nextId = Interlocked.Increment(ref lastId);

            savedValues.Add(nextId, Tuple.Create(frameId, value));

            return nextId;
        }

        public void Clear(long frameId)
        {
            savedValues
                .Where(kv => kv.Value.Item1 == frameId)
                .ToList()
                .ForEach(kv =>
                {
                    savedValues.Remove(kv);
                    kv.Value.Item2.Dispose();
                });
        }

        public void Dispose()
        {
            savedValues
                .Select(kv => kv.Value.Item2)
                .ToList()
                .ForEach(a => a.Dispose());

            savedValues.Clear();
        }
    }

    public class SavedValueFactory<TValue> : SavedValueRegistry<TValue>
        where TValue : IDisposable
    {
        private readonly Func<TValue> factory;

        public SavedValueFactory(Func<TValue> factory)
        {
            this.factory = factory;
        }

        public int Save(long frameId, out TValue value)
        {
            value = factory();
            return Save(frameId, value);
        }
    }
}
