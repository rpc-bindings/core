using DSerfozo.RpcBindings.Contract;
using DSerfozo.RpcBindings.Contract.Analyze;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Analyze
{
    public class ObjectAnalyzer : IObjectAnalyzer
    {
        private readonly IIdGenerator idGenerator;
        private readonly IPropertyAnalyzer propertyAnalyzer;
        private readonly IMethodAnalyzer methodAnalyzer;

        public ObjectAnalyzer(IIdGenerator idGenerator, IPropertyAnalyzer propertyAnalyzer, IMethodAnalyzer methodAnalyzer)
        {
            this.idGenerator = idGenerator;
            this.propertyAnalyzer = propertyAnalyzer;
            this.methodAnalyzer = methodAnalyzer;
        }

        public ObjectDescriptor AnalyzeObject(object o, AnalyzeOptions options)
        {
            var type = o.GetType();
            var builder = ObjectDescriptor.Create()
                .WithId(idGenerator.GetNextId())
                .WithMethods(methodAnalyzer.AnalyzeMethods(type))
                .WithName(options.Name)
                .WithObject(o);

            if (options.AnalyzeProperties)
            {
                builder.WithProperties(propertyAnalyzer.AnalyzeProperties(type, o, options.ExtractPropertyValues));
            }

            return builder.Get();
        }
    }
}
