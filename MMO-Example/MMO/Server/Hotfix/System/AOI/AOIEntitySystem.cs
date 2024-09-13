using Fantasy;
using Fantasy.Entitas.Interface;
using Fantasy.Event;

namespace Hotfix;

public sealed class AOIEntityAwakeSystem : AwakeSystem<AOIEntity>
{
    protected override void Awake(AOIEntity self)
    {
        self.ViewDistance = 10;
    }
}

public sealed class AOIEntityDestroySystem : DestroySystem<AOIEntity>
{
    protected override void Destroy(AOIEntity self)
    {
        self.Scene.GetComponent<AOIComponent>().Remove(self);
        self.ViewDistance = 0;
        self.SeeUnits.Clear();
        self.SeePlayers.Clear();
        self.BeSeePlayers.Clear();
        self.BeSeeUnits.Clear();
        self.SubEnterCells.Clear();
        self.SubLeaveCells.Clear();
        self.Cell = null;
    }
}

public static class AOIEntitySystem
{
    // public static void EnterSightAndNotifyClient(this AOIEntity self, AOIEntity other)
    // {
    //     Log.Debug("通知其他玩家有新玩家进入视野");
    //     self.SeeUnit.Add(other.Id, other); //自己可以看见的单位
    //     self.BeSeeUnit.Add(other.Id, other); //被别人看见
    //     other.SeeUnit.Add(self.Id, self); //别人看见的单位
    //     other.BeSeeUnit.Add(self.Id, self); //别人被看见的单位
    //
    //     // 通知其他玩家
    //     Room2C_CreateUnitMsg msg = new Room2C_CreateUnitMsg();
    //     msg.UnitInfos.Add(self.Unit.RoleInfo);
    //     MessageHelper.SendInnerRoute(self.Scene, other.Unit.SessionRuntimeId, msg);
    // }
    //
    // public static void Register(this AOIEntity self, List<Unit> units)
    // {
    //     // 进入视野
    //     foreach (Unit other in units)
    //     {
    //         // 排除自己
    //         if (other.Id == self.Unit.Id)
    //         {
    //             continue;
    //         }
    //
    //         Log.Debug($"Unit {other.RoleInfo.AccoutId} 进入视野");
    //         self.EnterSightAndNotifyClient(other.GetComponent<AOIEntity>());
    //         // unit.GetComponent<AOIEntity>().EnterOtherUnitSight(self); //被他人看见
    //     }
    //     // self.NotifyEnterSight();
    // }

    // cell中的unit进入self的视野
    public static void SubEnter(this AOIEntity self, Cell cell)
    {
        cell.SubsEnterEntities.Add(self.Id, self);
        foreach (KeyValuePair<long, AOIEntity> kv in cell.Units)
        {
            if (kv.Key == self.Id)
            {
                continue;
            }

            self.EnterSight(kv.Value);
        }
    }

    // cell中的unit离开self的视野
    public static void UnSubLeave(this AOIEntity self, Cell cell)
    {
        foreach (KeyValuePair<long, AOIEntity> kv in cell.Units)
        {
            if (kv.Key == self.Id)
            {
                continue;
            }

            self.LeaveSight(kv.Value);
        }

        cell.SubsLeaveEntities.Remove(self.Id);
    }

    /// <summary>
    /// unit进入self
    /// </summary>
    /// <param name="self"></param>
    /// <param name="cell"></param>
    /// <param name="list"></param>
    public static void SubEnter(this AOIEntity self, Cell cell, List<AOIEntity> list)
    {
        cell.SubsEnterEntities.Add(self.Id, self);

        foreach (var (key, aoiEntity) in cell.Units)
        {
            if (key == self.Id || aoiEntity.RunTimeId == 0)
            {
                continue;
            }

            var b = self.EnterSight(aoiEntity);

            if (b)
            {
                list.Add(aoiEntity);
            }
        }
    }

    public static void UnSubEnter(this AOIEntity self, Cell cell)
    {
        cell.SubsEnterEntities.Remove(self.Id);
    }

    public static void SubLeave(this AOIEntity self, Cell cell)
    {
        cell.SubsLeaveEntities.Add(self.Id, self);
    }

    //获取在自己视野中的对象
    public static Dictionary<long, AOIEntity> GetSeeUnits(this AOIEntity self)
    {
        return self.SeeUnits;
    }

    public static Dictionary<long, AOIEntity> GetBeSeePlayers(this AOIEntity self)
    {
        return self.BeSeePlayers;
    }

    public static Dictionary<long, AOIEntity> GetSeePlayers(this AOIEntity self)
    {
        return self.SeePlayers;
    }

    /// <summary>
    /// unit离开self视野
    /// </summary>
    /// <param name="self"></param>
    /// <param name="cell"></param>
    /// <param name="list"></param>
    public static void UnSubLeave(this AOIEntity self, Cell cell, List<AOIEntity> list)
    {
        foreach (var (key, aoiEntity) in cell.Units)
        {
            if (key == self.Id)
            {
                continue;
            }

            bool b = self.LeaveSight(aoiEntity);

            if (b)
            {
                list.Add(aoiEntity);
            }
        }

        cell.SubsLeaveEntities.Remove(self.Id);
    }

    /// <summary>
    /// unit进入self视野
    /// </summary>
    /// <param name="self"></param>
    /// <param name="enter"></param>
    public static bool EnterSight(this AOIEntity self, AOIEntity enter)
    {
        // 有可能之前在Enter，后来出了Enter还在LeaveCell，这样仍然没有删除，继续进来Enter，这种情况不需要处理

        if (self.Id == enter.Id || self.SeeUnits.ContainsKey(enter.Id))
        {
            return false;
        }

        if (self.Unit.UnitType == UnitType.Player)
        {
            self.SeeUnits.Add(enter.Id, enter);
            enter.BeSeeUnits.Add(self.Id, self);
            enter.BeSeePlayers.Add(self.Id, self);

            if (enter.Unit.UnitType == UnitType.Player)
            {
                self.SeePlayers.Add(enter.Id, enter);
            }
        }
        else
        {
            self.SeeUnits.Add(enter.Id, enter);
            enter.BeSeeUnits.Add(self.Id, self);

            if (enter.Unit.UnitType == UnitType.Player)
            {
                self.SeePlayers.Add(enter.Id, enter);
            }
        }

        self.Scene.EventComponent.Publish(new UnitEnterSightRange
        {
            Unit = self,
            Enter = enter
        });

        return true;
    }

    /// <summary>
    /// unit离开self视野
    /// </summary>
    /// <param name="self"></param>
    /// <param name="leave"></param>
    public static bool LeaveSight(this AOIEntity self, AOIEntity leave)
    {
        if (self.Id == leave.Id || !self.SeeUnits.ContainsKey(leave.Id))
        {
            return false;
        }

        self.SeeUnits.Remove(leave.Id);

        if (leave.Unit.UnitType == UnitType.Player)
        {
            self.SeePlayers.Remove(leave.Id);
        }

        leave.BeSeeUnits.Remove(self.Id);

        if (self.Unit.UnitType == UnitType.Player)
        {
            leave.BeSeePlayers.Remove(self.Id);
        }

        self.Scene.EventComponent.Publish(new UnitLeaveSightRange
        {
            Unit = self,
            Leave = leave
        });

        return true;
    }
}