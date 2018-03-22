using DSerfozo.RpcBindings.Analyze;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Tests.Fixtures;
using Moq;
using System.Linq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Analyze
{
    public class PropertyAnalyzerTests
    {
        [Fact]
        public void IdsSetForProperties()
        {
            PropertyAnalyzer propertyAnalyzer = new PropertyAnalyzer(new IntIdGenerator(), Mock.Of<IPropertyNameGenerator>());
            var actual = propertyAnalyzer.AnalyzeProperties(typeof(SimpleClassWithPrimitiveProperties), null, false).ToList();

            Assert.Collection(actual, f => Assert.Equal(1, f.Id), f => Assert.Equal(2, f.Id), f => Assert.Equal(3, f.Id));
        }

        [Fact]
        public void NamesSetForProperties()
        {
            PropertyAnalyzer propertyAnalyzer = new PropertyAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = propertyAnalyzer.AnalyzeProperties(typeof(SimpleClassWithPrimitiveProperties), null, false).ToList();

            Assert.Collection(actual,
                f => Assert.Equal(nameof(SimpleClassWithPrimitiveProperties.IntProperty), f.Name),
                f => Assert.Equal(nameof(SimpleClassWithPrimitiveProperties.DoubleProperty), f.Name),
                f => Assert.Equal(nameof(SimpleClassWithPrimitiveProperties.StringProperty), f.Name));
        }

        [Fact]
        public void IsReadOnlySet()
        {
            PropertyAnalyzer propertyAnalyzer = new PropertyAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = propertyAnalyzer.AnalyzeProperties(typeof(SimpleClassWithPrimitiveProperties), null, false).ToList();

            Assert.False(actual.Single(f => f.Name == nameof(SimpleClassWithPrimitiveProperties.StringProperty)).Writable);
        }

        [Fact]
        public void GettersSet()
        {
            PropertyAnalyzer propertyAnalyzer = new PropertyAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = propertyAnalyzer.AnalyzeProperties(typeof(SimpleClassWithPrimitiveProperties), null, false).ToList();

            const int IntValue = 3;
            const double DoubleValue = 3.0;
            const string StringValue = "string";
            var obj = new SimpleClassWithPrimitiveProperties(StringValue)
            {
                IntProperty = IntValue,
                DoubleProperty = DoubleValue
            };

            Assert.Equal(IntValue, actual[0].Getter(obj));
            Assert.Equal(DoubleValue, actual[1].Getter(obj));
            Assert.Equal(StringValue, actual[2].Getter(obj));
        }

        [Fact]
        public void SettersSet()
        {
            PropertyAnalyzer propertyAnalyzer = new PropertyAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = propertyAnalyzer.AnalyzeProperties(typeof(SimpleClassWithPrimitiveProperties), null, false).ToList();

            const int IntValue = 3;
            const double DoubleValue = 3.0;
            var obj = new SimpleClassWithPrimitiveProperties("str")
            {
            };

            actual[0].Setter(obj, IntValue);
            actual[1].Setter(obj, DoubleValue);

            Assert.Equal(IntValue, obj.IntProperty);
            Assert.Equal(DoubleValue, obj.DoubleProperty);
        }

        [Fact]
        public void IgnoredPropertySkipped()
        {
            PropertyAnalyzer propertyAnalyzer = new PropertyAnalyzer(new IntIdGenerator(), Mock.Of<IPropertyNameGenerator>());
            var actual = propertyAnalyzer.AnalyzeProperties(typeof(SimpleClassIgnore), null, false).ToList();

            Assert.Equal(1, actual.Count);
        }

        [Fact]
        public void IndexerSkipped()
        {
            PropertyAnalyzer propertyAnalyzer = new PropertyAnalyzer(new IntIdGenerator(), Mock.Of<IPropertyNameGenerator>());
            var actual = propertyAnalyzer.AnalyzeProperties(typeof(SimpleClassWithIndexer), null, false).ToList();

            Assert.Equal(0, actual.Count);
        }

        [Fact]
        public void PropertyValueAdded()
        {
            const int IntValue = 3;
            const double DoubleValue = 3.0;
            const string StringValue = "string";
            var obj = new SimpleClassWithPrimitiveProperties(StringValue)
            {
                IntProperty = IntValue,
                DoubleProperty = DoubleValue
            };

            PropertyAnalyzer propertyAnalyzer = new PropertyAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = propertyAnalyzer.AnalyzeProperties(typeof(SimpleClassWithPrimitiveProperties), obj, true).ToList();

            Assert.Equal(IntValue, actual[0].Value);
            Assert.Equal(DoubleValue, actual[1].Value);
            Assert.Equal(StringValue, actual[2].Value);
        }
    }
}
