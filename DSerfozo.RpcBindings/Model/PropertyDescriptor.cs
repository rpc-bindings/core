using Newtonsoft.Json;
using System;

namespace DSerfozo.RpcBindings.Model
{
    public sealed class PropertyDescriptor
    {
        public sealed class Builder
        {
            private readonly PropertyDescriptor constructed = new PropertyDescriptor();

            public Builder WithId(int id)
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
                constructed.IsReadOnly = readOnly;
                return this;
            }

            public PropertyDescriptor Get()
            {
                return constructed;
            }
        }

        [JsonProperty("id")]
        public int Id { get; private set;  }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("isReadOnly")]
        public bool IsReadOnly { get; private set; }

        public Func<object, object> Getter { get; private set; }

        public Action<object, object> Setter { get; private set; }

        private PropertyDescriptor()
        {
            
        }

        public static Builder Create()
        {
            return new Builder();
        }
    }
}
