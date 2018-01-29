using System.Collections.Generic;
using System.Linq;
using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Model
{
    public sealed class ObjectDescriptor
    {
        public sealed class Builder
        {
            private readonly ObjectDescriptor constructed = new ObjectDescriptor();

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

            public Builder WithProperties(IEnumerable<PropertyDescriptor> properties)
            {
                constructed.Properties = properties.GroupBy(p => p.Id).ToDictionary(p => p.Key, p => p.Single());

                return this;
            }

            public Builder WithMethods(IEnumerable<MethodDescriptor> methods)
            {
                constructed.Methods = methods.GroupBy(p => p.Id).ToDictionary(p => p.Key, p => p.Single());

                return this;
            }

            public Builder WithObject(object obj)
            {
                constructed.Object = obj;

                return this;
            }

            public ObjectDescriptor Get()
            {
                return constructed;
            }

            internal Builder()
            {

            }
        }

        [ShouldSerialize]
        public int Id { get; private set; }

        [ShouldSerialize]
        public string Name { get; private set; }

        [ShouldSerialize]
        public IDictionary<int, PropertyDescriptor> Properties { get; private set; }

        [ShouldSerialize]
        public IDictionary<int, MethodDescriptor> Methods { get; private set; }

        public object Object { get; private set; }

        private ObjectDescriptor()
        {
        }

        public static Builder Create()
        {
            return new Builder();
        }
    }
}
