using Fantasy.Async;
using Fantasy.Network.Interface;
using Fantasy.PacketParser;
using Fantasy.PacketParser.Interface;
using Fantasy.Scheduler;
using Fantasy.Serialize;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#if FANTASY_NET
namespace Fantasy.Network;

/// <summary>
/// 网络服务器内部会话。
/// </summary>
public sealed class ProcessSession : Session
{
    /// <summary>
    /// 发送消息到服务器内部。
    /// </summary>
    /// <param name="message">要发送的消息。</param>
    /// <param name="rpcId">RPC 标识符。</param>
    /// <param name="routeId">路由标识符。</param>
    public override void Send(IMessage message, uint rpcId = 0, long routeId = 0)
    {
        if (IsDisposed)
        {
            return;
        }

        this.Scheduler(message.GetType(), rpcId, routeId, message.OpCode(), message);
    }

    /// <summary>
    /// 发送路由消息到服务器内部。
    /// </summary>
    /// <param name="routeMessage">要发送的路由消息。</param>
    /// <param name="rpcId">RPC 标识符。</param>
    /// <param name="routeId">路由标识符。</param>
    public override void Send(IRouteMessage routeMessage, uint rpcId = 0, long routeId = 0)
    {
        if (IsDisposed)
        {
            return;
        }
        
        this.Scheduler(routeMessage.GetType(), rpcId, routeId, routeMessage.OpCode(), routeMessage);
    }

    public override void Send(uint rpcId, long routeId, Type messageType, APackInfo packInfo)
    {
        if (IsDisposed)
        {
            return;
        }

        this.Scheduler(messageType, rpcId, routeId, packInfo);
    }

    public override void Send(ProcessPackInfo packInfo, uint rpcId = 0, long routeId = 0)
    {
        this.Scheduler(packInfo.MessageType, rpcId, routeId, packInfo);
    }

    public override void Send(MemoryStreamBuffer memoryStream, uint rpcId = 0, long routeId = 0)
    {
        throw new Exception("The use of this method is not supported");
    }

    public override FTask<IResponse> Call(IRouteRequest request, long routeId = 0)
    {
        throw new Exception("The use of this method is not supported");
    }

    public override FTask<IResponse> Call(IRequest request, long routeId = 0)
    {
        throw new Exception("The use of this method is not supported");
    }
}
#endif