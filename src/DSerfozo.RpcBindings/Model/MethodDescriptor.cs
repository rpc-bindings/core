using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSerfozo.RpcBindings.Contract;

namespace DSerfozo.RpcBindings.Model
{
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

            public MethodDescriptor Get()
            {
                return constructed;
            }

            internal Builder()
            {
                constructed = new MethodDescriptor();
            }
        }

        [ShouldSerialize]
        public long Id { get; private set;  }

        [ShouldSerialize]
        public string Name { get; private set; }

        public Type ResultType { get; private set; }

        [ShouldSerialize]
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
