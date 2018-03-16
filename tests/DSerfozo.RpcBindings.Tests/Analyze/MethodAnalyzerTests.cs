using DSerfozo.RpcBindings.Analyze;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Tests.Fixtures;
using Moq;
using System.Linq;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Analyze
{
    public class MethodAnalyzerTests
    {
        [Fact]
        public void IdsSetForMethods()
        {
            MethodAnalyzer methodAnalyzer = new MethodAnalyzer(new IntIdGenerator(), Mock.Of<IMethodNameGenerator>());
            var actual = methodAnalyzer.AnalyzeMethods(typeof(SimpleClassWithMethods)).ToList();

            Assert.Collection(actual, f => Assert.Equal(1, f.Id), f => Assert.Equal(2, f.Id), f => Assert.Equal(3, f.Id));
        }

        [Fact]
        public void NamesSetForMethods()
        {
            MethodAnalyzer methodAnalyzer = new MethodAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = methodAnalyzer.AnalyzeMethods(typeof(SimpleClassWithMethods)).ToList();

            Assert.Collection(actual,
                f => Assert.Equal(nameof(SimpleClassWithMethods.MethodWithoutReturn), f.Name),
                f => Assert.Equal(nameof(SimpleClassWithMethods.MethodWithParameters), f.Name),
                f => Assert.Equal(nameof(SimpleClassWithMethods.MethodWithResult), f.Name));
        }

        [Fact]
        public void ParameterCountSetForMethods()
        {
            MethodAnalyzer methodAnalyzer = new MethodAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = methodAnalyzer.AnalyzeMethods(typeof(SimpleClassWithMethods)).ToList();

            Assert.Collection(actual.OrderBy(p => p.ParameterCount),
                f => Assert.Equal(0, f.ParameterCount),
                f => Assert.Equal(1, f.ParameterCount),
                f => Assert.Equal(3, f.ParameterCount));
        }

        [Fact]
        public void ParameterInformationSetForMethods()
        {
            MethodAnalyzer methodAnalyzer = new MethodAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = methodAnalyzer.AnalyzeMethods(typeof(SimpleClassWithMethods)).ToList();

            Assert.Collection(actual.OrderBy(p =>p.ParameterCount),
                f => Assert.Empty(f.Parameters),
                f => Assert.Collection(f.Parameters, p => Assert.Equal(typeof(string), p.Type)),
                f => Assert.Collection(f.Parameters,
                    p=> Assert.Equal(typeof(int), p.Type),
                    p => Assert.Equal(typeof(double), p.Type),
                    p => Assert.Equal(typeof(string), p.Type)));
        }

        [Fact]
        public void ResultTypeSetForMethods()
        {
            MethodAnalyzer methodAnalyzer = new MethodAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = methodAnalyzer.AnalyzeMethods(typeof(SimpleClassWithMethods)).ToList();

            Assert.Collection(actual.OrderBy(p => p.ParameterCount),
                f => Assert.Equal(typeof(void), f.ResultType),
                f => Assert.Equal(typeof(string), f.ResultType),
                f => Assert.Equal(typeof(void), f.ResultType));
        }

        [Fact]
        public void IgnoredMethodsSkipped()
        {
            MethodAnalyzer methodAnalyzer = new MethodAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = methodAnalyzer.AnalyzeMethods(typeof(SimpleClassIgnore)).ToList();

            Assert.Equal(1, actual.Count);
        }

        [Fact]
        public void ExecuteSetForMethods()
        {
            const int intValue = 1;
            const double doubleValue = 3.0;
            const string stringValue = "str";

            var mock = new Mock<SimpleClassWithMethods>();
            mock.Setup(m => m.MethodWithResult(stringValue)).Returns(stringValue);

            MethodAnalyzer methodAnalyzer = new MethodAnalyzer(new IntIdGenerator(), new IdentityNameGenerator());
            var actual = methodAnalyzer.AnalyzeMethods(typeof(SimpleClassWithMethods)).ToList();

            actual[0].Execute(mock.Object, new object[] { });
            mock.Verify(m => m.MethodWithoutReturn(), Times.Once);

            actual[1].Execute(mock.Object, new object[] { intValue, doubleValue, stringValue });
            mock.Verify(m => m.MethodWithParameters(intValue, doubleValue, stringValue), Times.Once);

            Assert.Equal(stringValue, actual[2].Execute(mock.Object, new object[] { stringValue }));
        }
    }
}
