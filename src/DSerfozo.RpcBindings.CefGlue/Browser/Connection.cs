using System;
using System.Reactive.Subjects;
using DSerfozo.RpcBindings.CefGlue.Common;
using DSerfozo.RpcBindings.CefGlue.Common.Serialization;
using DSerfozo.RpcBindings.Contract.Communication;
using DSerfozo.RpcBindings.Contract.Communication.Model;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Browser
{
    public class Connection : IConnection<CefValue>, IDisposable
    {
        private readonly ISubject<RpcResponse<CefValue>> rpcResponseSubject = new Subject<RpcResponse<CefValue>>();
        private CefBrowser browser;
        private MessageClient client;

        public bool IsOpen => browser != null;

        public void Initialize(CefBrowser browser, MessageClient client)
        {
            this.browser = browser;
            this.client = client;

            client.ProcessMessageReceived += ClientOnProcessMessageReceived;
        }

        private void ClientOnProcessMessageReceived(object sender, ProcessMessageReceivedArgs e)
        {
            var message = e.Message;
            var rpcResponse = RpcMessageSerializer.CreateRpcResponse(message);

            if (rpcResponse != null)
            {
                e.Handled = true;
                rpcResponseSubject.OnNext(rpcResponse);
            }
        }

        public void Send(RpcRequest<CefValue> rpcRequest)
        {
            var message = rpcRequest.ToCefProcessMessage();
            if (message != null)
            {
                browser.SendProcessMessage(CefProcessId.Renderer, message);
            }
            else
            {
                throw new Exception();
            }
        }

        public void Dispose()
        {
            client.ProcessMessageReceived -= ClientOnProcessMessageReceived;
        }

        public IDisposable Subscribe(IObserver<RpcResponse<CefValue>> observer)
        {
            return rpcResponseSubject.Subscribe(observer);
        }
    }
}
