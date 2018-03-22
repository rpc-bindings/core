using System;
using System.ComponentModel;
using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Model
{
    public sealed class PropertyDescriptor
    {
        public sealed class Builder
        {
            private readonly PropertyDescriptor constructed = new PropertyDescriptor()
            {
                Readable = true,
                Writable = true
            };

            public Builder WithId(long id)
            {
                constructed.Id = id;
                return this;
            }

            public Builder WithName(string name)
            {
                constructed.Name = name;
                return this;
            }

            public Builder WithGetter(Func<object, object> getter)
            {
                constructed.Getter = getter;
                return this;
            }

            public Builder WithSetter(Action<object, object> setter)
            {
                constructed.Setter = setter;
                return this;
            }

            public Builder WithReadOnly(bool readOnly)
            {
                constructed.Writable = !readOnly;
                return this;
            }

            public Builder WithType(Type type)
            {
                constructed.Type = type;
                return this;
            }

            public Builder WithValue(object value)
            {
                constructed.Value = value;
                return this;
            }

            public PropertyDescriptor Get()
            {
                return constructed;
            }
        }

        [ShouldSerialize]
        public long Id { get; private set;  }

        [ShouldSerialize]
        public string Name { get; private set; }

        [ShouldSerialize]
        public bool Readable { get; private set; }

        [ShouldSerialize]
        public bool Writable { get; private set; }

        [ShouldSerialize]
        public object Value { get; private set; }

        public Func<object, object> Getter { get; private set; }

        public Action<object, object> Setter { get; private set; }

        public Type Type { get; private set; }

        private PropertyDescriptor()
        {
            
        }

        public static Builder Create()
        {
            return new Builder();
        }
    }
}
