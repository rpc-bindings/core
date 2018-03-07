using System;
using DSerfozo.RpcBindings.Contract.Communication.Model;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IConnection<TMarshal> : IObservable<RpcResponse<TMarshal>>
    {
        bool IsOpen { get; }

        void Send(RpcRequest<TMarshal> rpcRequest);
    }
}
