using DSerfozo.RpcBindings.Analyze;
using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Model;
using DSerfozo.RpcBindings.Tests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using DSerfozo.RpcBindings.Contract.Analyze;
using Xunit;

namespace DSerfozo.RpcBindings.Tests.Analyze
{
    public class ObjectAnalyzerTests
    {
        [Fact]
        public void IdSetOnObjectDescriptor()
        {
            const int Id = 1;

            var idGeneratorMock = new Mock<IIdGenerator>();
            idGeneratorMock.Setup(_ => _.GetNextId()).Returns(Id);
            var objectAnalyzer = new ObjectAnalyzer(idGeneratorMock.Object, Mock.Of<IPropertyAnalyzer>(), Mock.Of<IMethodAnalyzer>());
            var descriptor = objectAnalyzer.AnalyzeObject(new SimpleClass(), new AnalyzeOptions()
            {
                Name = "name"
            });

            Assert.True(descriptor.Id == Id);
        }

        [Fact]
        public void NameSetOnObjectDescriptor()
        {
            const int Id = 1;
            const string Name = "name";

            var idGeneratorMock = new Mock<IIdGenerator>();
            idGeneratorMock.Setup(_ => _.GetNextId()).Returns(Id);
            var objectAnalyzer = new ObjectAnalyzer(idGeneratorMock.Object, Mock.Of<IPropertyAnalyzer>(), Mock.Of<IMethodAnalyzer>());
            var descriptor = objectAnalyzer.AnalyzeObject(new SimpleClass(), new AnalyzeOptions()
            {
                Name = Name
            });

            Assert.True(descriptor.Name == Name);
        }

        [Fact]
        public void ObjectSetOnObjectDescriptor()
        {
            const int Id = 1;
            const string Name = "name";

            var idGeneratorMock = new Mock<IIdGenerator>();
            idGeneratorMock.Setup(_ => _.GetNextId()).Returns(Id);
            var objectAnalyzer = new ObjectAnalyzer(idGeneratorMock.Object, Mock.Of<IPropertyAnalyzer>(), Mock.Of<IMethodAnalyzer>());
            var o = new SimpleClass();
            var descriptor = objectAnalyzer.AnalyzeObject(o, new AnalyzeOptions()
            {
                Name = Name
            });

            Assert.Same(o, descriptor.Object);
        }

        [Fact]
        public void MethodsSetOnObjectDescriptor()
        {
            const int Id = 1;
            const string Name = "name";

            var idGeneratorMock = new Mock<IIdGenerator>();
            idGeneratorMock.Setup(_ => _.GetNextId()).Returns(Id);
            var methodAnalyzer = Mock.Of<IMethodAnalyzer>();
            var methodAnalyzerMock = Mock.Get(methodAnalyzer);
            List<MethodDescriptor> value = new List<MethodDescriptor>()
            {
                MethodDescriptor.Create().WithId(1).Get()
            };
            methodAnalyzerMock.Setup(_ => _.AnalyzeMethods(It.IsAny<Type>())).Returns(value);

            var objectAnalyzer = new ObjectAnalyzer(idGeneratorMock.Object, Mock.Of<IPropertyAnalyzer>(), methodAnalyzer);
            var descriptor = objectAnalyzer.AnalyzeObject(new SimpleClass(), new AnalyzeOptions()
            {
                Name = Name
            });

            Assert.Collection(descriptor.Methods, m => Assert.True(m.Key == 1 && m.Value.Id == 1));
        }

        [Fact]
        public void PropertiesSetOnObjectDescriptor()
        {
            const int Id = 1;
            const string Name = "name";

            var idGeneratorMock = new Mock<IIdGenerator>();
            idGeneratorMock.Setup(_ => _.GetNextId()).Returns(Id);
            var propertyAnalyzer = Mock.Of<IPropertyAnalyzer>();
            var propertyAnalyzerMock = Mock.Get(propertyAnalyzer);
            List<PropertyDescriptor> value = new List<PropertyDescriptor>()
            {
                PropertyDescriptor.Create().WithId(1).Get()
            };
            propertyAnalyzerMock.Setup(_ => _.AnalyzeProperties(typeof(SimpleClassWithPrimitiveProperties),
                It.IsAny<SimpleClassWithPrimitiveProperties>(), true)).Returns(value);

            var objectAnalyzer = new ObjectAnalyzer(idGeneratorMock.Object, propertyAnalyzer, Mock.Of<IMethodAnalyzer>());
            var descriptor = objectAnalyzer.AnalyzeObject(new SimpleClassWithPrimitiveProperties("str"), new AnalyzeOptions()
            {
                Name = "name",
                AnalyzeProperties = true,
                ExtractPropertyValues = true
            });

            Assert.Collection(descriptor.Properties, m => Assert.True(m.Key == 1 && m.Value.Id == 1));
        }
    }
}
