using DSerfozo.RpcBindings.Contract.Analyze;
using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IObjectAnalyzer
    {
        ObjectDescriptor AnalyzeObject(object o, AnalyzeOptions options);
    }
}