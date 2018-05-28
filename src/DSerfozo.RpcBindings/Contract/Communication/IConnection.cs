﻿using System;
using DSerfozo.RpcBindings.Contract.Communication.Model;

namespace DSerfozo.RpcBindings.Contract.Communication
{
    public interface IConnection<TMarshal> : IConnectionAvailability, IObservable<RpcResponse<TMarshal>>
    {
        void Send(RpcRequest<TMarshal> rpcRequest);
    }
}
