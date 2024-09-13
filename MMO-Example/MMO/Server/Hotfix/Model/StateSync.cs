using Fantasy;
using Fantasy.Async;
using Fantasy.DataStructure.Collection;
using Fantasy.Entitas;
using Fantasy.Network.Interface;
using Fantasy.Serialize;

namespace BestGame;

public class StateSync : Entity 
{
    //public IMessage Message;
    public OneToManyList<long, IProto> mDict = new OneToManyList<long, IProto>();
    public long Interval = 15;
    public bool IsWait;

    public void Clear()
    {
        mDict.Clear();
        IsWait = false;
    }

    public void AddMessage(long unitId, IProto stateInfo)
    {
        mDict.Add(unitId, stateInfo);

        if (IsWait) return;
        AddSync().Coroutine();
    }

    public async FTask AddSync()
    {
        IsWait = true;
        long runtimeId = RunTimeId;

        // 延迟15毫秒发送，让mDict收集数据
        await Scene.TimerComponent.Net.WaitAsync(Interval);
        
        // 防止异步的时候自己销毁了、下面逻辑执行的不对
        if (runtimeId != RunTimeId) return;
        
        IsWait = false;
        Send();
    }

    public virtual void Send()
    {
        
    }

    public void SendMessage(IMessage message)
    {
        var unit = (Unit)Parent;
        
        if(!unit.IsDisposed)
           unit.Scene.NetworkMessagingComponent.SendInnerRoute(unit.SessionRuntimeId,
                (IRouteMessage)message
            );
        
        mDict.Clear();
    }
}