using DSerfozo.RpcBindings.Communication;
using DSerfozo.RpcBindings.Model;
using System;
using System.Threading.Tasks;

namespace DSerfozo.RpcBindings.Contract
{
    public interface IConnection<TMarshal>
    {
        event Action<RpcResponse<TMarshal>> RpcResponse;

        Task Send(RpcRequest<TMarshal> rpcRequest);
    }
}
