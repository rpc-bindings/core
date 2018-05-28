using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Model
{
    [DataContract]
    [TypeId(TypeId)]
    public sealed class ObjectDescriptor
    {
        public const string TypeId = "5F6FA749-5CD1-4A51-9F69-0B9657C55ECC";

        public sealed class Builder
        {
            private readonly ObjectDescriptor constructed = new ObjectDescriptor();

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

        [DataMember] public string Type { get; private set; } = TypeId;

        [DataMember]
        public long Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public IDictionary<long, PropertyDescriptor> Properties { get; private set; }

        [DataMember]
        public IDictionary<long, MethodDescriptor> Methods { get; private set; }

        public object Object { get; private set; }

        public static Builder Create()
        {
            return new Builder();
        }
    }
}
