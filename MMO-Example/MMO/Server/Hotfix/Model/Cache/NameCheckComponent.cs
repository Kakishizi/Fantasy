using Fantasy;
using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Entitas.Interface;
using Fantasy.Model;

namespace BestGame;

public class NameCheckComponentDestroySystem : DestroySystem<NameCheckComponent>
{
    protected override void Destroy(NameCheckComponent self)
    {
        self.Names.Clear();
    }
}

public class NameCheckComponent : Entity
{
    public HashSet<string> Names = new HashSet<string>();

    public async FTask<uint> CanUse(string name)
    {
        using (await Scene.CoroutineLockComponent.Wait(LockType.LockRoleName,name.GetHashCode(),nameof(LockType.LockRoleName)))
        {
            // 已经占用
            if (Names.Contains(name))
                return ErrorCode.Error_NameCheckIsUse;

            var v = await Scene.World.DateBase.Query<Role>(b => b.NickName == name);

            if (v != null && v.Count > 0)
                return ErrorCode.Error_NameCheckIsUse;
            
            // 添加到已用列表中
            Names.Add(name);
            
            return ErrorCode.Success;
        }
    }
}