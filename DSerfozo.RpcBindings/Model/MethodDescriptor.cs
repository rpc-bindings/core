using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DSerfozo.RpcBindings.Model
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class MethodDescriptor
    {
        public sealed class Builder
        {
            private readonly MethodDescriptor constructed;

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

        [JsonProperty("id")]
        public int Id { get; private set;  }

        [JsonProperty("name")]
        public string Name { get; private set; }

        public Type ResultType { get; private set; }

        [JsonProperty("parameterCount")]
        public int ParameterCount { get; private set; }

        public IEnumerable<MethodParameterDescriptor> Parameters { get; private set; }

        public Func<object, object[], object> Execute { get; private set; }

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
