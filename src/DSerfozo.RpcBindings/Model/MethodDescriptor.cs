using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Marshaling;

namespace DSerfozo.RpcBindings.Model
{
    [DataContract]
    public sealed class MethodDescriptor
    {
        public sealed class Builder
        {
            private readonly MethodDescriptor constructed;

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

            public Builder WithExecute(Func<object, object[], object> execute)
            {
                constructed.Execute = execute;
                return this;
            }

            public Builder WithParameterCount(int parameterCount)
            {
                constructed.ParameterCount = parameterCount;
                return this;
            }

            public Builder WithParameters(IEnumerable<MethodParameterDescriptor> parameters)
            {
                constructed.Parameters = parameters;
                return this;
            }

            public Builder WithResultType(Type resultType)
            {
                constructed.ResultType = resultType;
                return this;
            }

            public Builder WithBindValue(BindValueAttribute attr)
            {
                constructed.ReturnValueAttribute = attr;
                return this;
            }

            public MethodDescriptor Get()
            {
                return constructed;
            }

            internal Builder()
            {
                constructed = new MethodDescriptor();
            }
        }

        [DataMember]
        public long Id { get; private set;  }

        [DataMember]
        public string Name { get; private set; }

        public Type ResultType { get; private set; }

        public BindValueAttribute ReturnValueAttribute { get; private set; }

        [DataMember]
        public int ParameterCount { get; private set; }

        public IEnumerable<MethodParameterDescriptor> Parameters { get; private set; }

        public Func<object, object[], object> Execute { get; private set; }

        public bool IsAsync => typeof(Task).IsAssignableFrom(ResultType);

        public MethodDescriptor Clone()
        {
            return MemberwiseClone() as MethodDescriptor;
        }

        public static Builder Create()
        {
            return new Builder();
        }   
    }
}
