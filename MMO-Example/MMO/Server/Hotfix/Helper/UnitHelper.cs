using Hotfix;

namespace Fantasy;

public static class UnitHelper
{


    /// <summary>
    /// 创建UnitInfo
    /// </summary>
    public static RoleInfo CreateUnitInfo(this Unit unit)
    {
        var info = unit.RoleInfo;
        info.LastMoveInfo = new MoveInfo(){
            Position = unit.Position.ToPosition(),
            Rotation = unit.Rotation.ToRotation(),
        };

        return info;
    }

    public static void FromeCache(this Unit unit, Unit cache)
    {
        unit.UnitType = cache.UnitType;
        unit.RoleInfo = cache.RoleInfo;
        unit.Position = cache.Position;
        unit.Rotation = cache.Rotation;
        // 还有其它需要缓存的组件数据
        // ...
    }

    public static void NoticeUnitAdd(Unit unit, Unit enter)
    {
        var m2cUnitCreate = new M2C_UnitCreate();
        var unitInfo = enter.CreateUnitInfo();
        m2cUnitCreate.UnitInfos.Add(unitInfo);

        // 通知客户端有新的Unit进入到视野中
        unit.Scene.NetworkMessagingComponent.SendInnerRoute(unit.SessionRuntimeId, m2cUnitCreate);
    }

    public static void NoticeUnitRemove(Unit unit, Unit leave)
    {
        var removeUnits = new M2C_UnitRemove();
        removeUnits.RoleIds.Add(leave.RoleInfo.RoleId);
        
        // 通知客户端有Unit离开视野
        unit.Scene.NetworkMessagingComponent.SendInnerRoute( unit.SessionRuntimeId, removeUnits);
    }

    public static int GetConfigIdByClassName( string className)
    {
        var unitConfigs = ConfigSystem.Instance.GetUnitConfigList();
         foreach (var config in unitConfigs)
         {
             if (config.ClassName == className)
                 return config.Id;
         }

        // 如果未找到匹配的对象，则返回默认值（例如 0）
        return 0;
    }

    public static int GetMapNumByUnitConfigId(int unitConfigId)
    {
        var unitConfig = ConfigSystem.Instance.GetUnitConfig(unitConfigId);
        return unitConfig.Id;
    }

    public static MoveInfo GetMoveInfoByUnitConfigId(int unitConfigId)
    {
        var unitConfig = ConfigSystem.Instance.GetUnitConfig(unitConfigId);
        var position = StringHelper.ParseCoordinates(unitConfig.Position);
        var rotation = StringHelper.ParseRotation(unitConfig.Angle);

        return MoveMessageHelper.MoveInfo(position, rotation);
    }
}