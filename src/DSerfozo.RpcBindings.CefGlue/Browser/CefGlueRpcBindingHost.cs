using System.Reactive.Concurrency;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Browser
{
    public class CefGlueRpcBindingHost : RpcBindingHost<CefValue>
    {
        public CefGlueRpcBindingHost(Connection connection) : base(connection,
            factory => new CefValueBinder(factory), new EventLoopScheduler())
        {
        }
    }
}
