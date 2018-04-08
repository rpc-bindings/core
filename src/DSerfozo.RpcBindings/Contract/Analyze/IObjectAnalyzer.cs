using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract.Analyze
{
    public interface IObjectAnalyzer
    {
        ObjectDescriptor AnalyzeObject(object o, AnalyzeOptions options);
    }
}