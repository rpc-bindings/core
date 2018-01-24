using DSerfozo.RpcBindings.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IObjectAnalyzer
    {
        ObjectDescriptor AnalyzeObject<TObject>(string name, TObject o);

        ObjectDescriptor AnalyzeObject(string name, object o);
    }
}