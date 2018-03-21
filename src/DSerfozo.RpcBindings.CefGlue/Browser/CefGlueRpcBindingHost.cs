using System.Reactive.Concurrency;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Browser
{
    public class CefGlueRpcBindingHost : RpcBindingHost<CefValue>
    {
        public CefGlueRpcBindingHost(Connection connection) : base(connection,
            new CefValueBinder(), new EventLoopScheduler())
        {
        }
    }
}
