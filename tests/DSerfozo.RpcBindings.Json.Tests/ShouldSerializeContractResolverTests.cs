using System.IO;
using DSerfozo.RpcBindings.Contract;
using Newtonsoft.Json;
using Xunit;

namespace DSerfozo.RpcBindings.Json.Tests
{
    public class ShouldSerializeContractResolverTests
    {
        private class TestData
        {
            [ShouldSerialize]
            public string Serialized { get; set; }

            [JsonProperty]
            public string SerializedToo { get; set; }

            public string NotSerialized { get; set; }
        }

        [Fact]
        public void CamelCase()
        {
            var resolver = new ShouldSerializeContractResolver();
            var serializer = new JsonSerializer()
            {
                ContractResolver = resolver
            };


            using (var strWriter = new StringWriter())
            {
                serializer.Serialize(strWriter, new TestData()
                {
                    Serialized = "serialized",
                    NotSerialized = "not-serialized"
                });

                var serialized = strWriter.ToString();

                Assert.Contains("\"serialized\":", serialized);
            }
        }

        [Fact]
        public void PropertyFilteringWorks()
        {
            var resolver = new ShouldSerializeContractResolver();
            var serializer = new JsonSerializer()
            {
                ContractResolver = resolver
            };

            using (var strWriter = new StringWriter())
            {
                serializer.Serialize(strWriter, new TestData()
                {
                    Serialized = "serialized",
                    NotSerialized = "not-serialized"
                });

                var serialized = strWriter.ToString();
                var deserialized = serializer.Deserialize<TestData>(new JsonTextReader(new StringReader(serialized)));

                Assert.Equal("serialized", deserialized.Serialized);
                Assert.Null(deserialized.NotSerialized);
            }
        }

        [Fact]
        public void PropertyFilteringWorksForJsonPropertyToo()
        {
            var resolver = new ShouldSerializeContractResolver();
            var serializer = new JsonSerializer()
            {
                ContractResolver = resolver
            };

            using (var strWriter = new StringWriter())
            {
                serializer.Serialize(strWriter, new TestData()
                {
                    Serialized = "serialized",
                    SerializedToo = "serialized",
                    NotSerialized = "not-serialized"
                });

                var serialized = strWriter.ToString();
                var deserialized = serializer.Deserialize<TestData>(new JsonTextReader(new StringReader(serialized)));

                Assert.Equal("serialized", deserialized.SerializedToo);
            }
        }
    }
}
