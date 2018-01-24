using DSerfozo.RpcBindings.Contract;
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

        public ObjectDescriptor AnalyzeObject<TObject>(string name, TObject o)
        {
            return ObjectDescriptor.Create()
                .WithId(idGenerator.GetNextId())
                .WithMethods(methodAnalyzer.AnalyzeMethods(typeof(TObject)))
                .WithObject(o)
                .WithName(name)
                .Get();
        }

        public ObjectDescriptor AnalyzeObject(string name, object o)
        {
            return ObjectDescriptor.Create()
                .WithId(idGenerator.GetNextId())
                .WithMethods(methodAnalyzer.AnalyzeMethods(o.GetType()))
                .WithObject(o)
                .WithName(name)
                .Get();
        }
    }
}
