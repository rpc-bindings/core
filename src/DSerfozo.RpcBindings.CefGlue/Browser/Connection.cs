﻿using System;
using System.Collections.Generic;
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
        private readonly ObjectSerializer objectSerializer;
        private CefBrowser browser;
        private MessageClient client;
        private bool browserDisposed;

        public bool IsOpen => browser != null && !browserDisposed;

        public Connection(ObjectSerializer objectSerializer)
        {
            this.objectSerializer = objectSerializer;
        }

        public void Initialize(CefBrowser browser, MessageClient client)
        {
            this.browser = browser;
            this.client = client;

            client.ProcessMessageReceived += ClientOnProcessMessageReceived;
        }

        private void ClientOnProcessMessageReceived(object sender, ProcessMessageReceivedArgs e)
        {
            var message = e.Message;

            if (message.Name == Messages.RpcResponseMessage)
            {
                var response = message.Arguments.GetValue(0);

                var rpcResponse =
                    (RpcResponse<CefValue>) objectSerializer.Deserialize(response, typeof(RpcResponse<CefValue>));

                if (rpcResponse != null)
                {
                    e.Handled = true;
                    rpcResponseSubject.OnNext(rpcResponse);
                }
            }
        }

        public void Send(RpcRequest<CefValue> rpcRequest)
        {
            if (browserDisposed)
                return;

            var message = CefProcessMessage.Create(Messages.RpcRequestMessage);
            try
            {
                message.Arguments.SetValue(0, objectSerializer.Serialize(rpcRequest, new HashSet<object>()));

                browser.SendProcessMessage(CefProcessId.Renderer, message);
            }
            catch (NullReferenceException)
            {
                //when the browser is already disposed
                //this can happen from a finalized callback for example
                browserDisposed = true;
                message.Dispose();
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
