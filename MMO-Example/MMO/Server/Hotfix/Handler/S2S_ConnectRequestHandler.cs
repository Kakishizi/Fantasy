using Fantasy;
using Fantasy.Async;
using Fantasy.Helper;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace BestGame;
public class S2S_ConnectRequestHandler : MessageRPC<S2S_ConnectRequest, S2S_ConnectResponse>
{
    // 内网连接Handler
    protected override async FTask Run(Session session, S2S_ConnectRequest request, S2S_ConnectResponse response, Action reply)
    {
        response.Key = (int)RandomHelper.RandUInt32();

        await FTask.CompletedTask;
    }
}


