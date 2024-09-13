using UnityEngine;
using System.Collections.Generic;
using Fantasy.DataStructure.Collection;

namespace Fantasy;

public static class AOIHelper
{
    /// <summary>
    /// 进入或离开Cell
    /// </summary>
    /// <param name="aoiEntity"></param>
    /// <param name="cellX"></param>
    /// <param name="cellY"></param>
    /// <param name="enterCell"></param>
    /// <param name="leaveCell"></param>
    public static void CalcEnterAndLeaveCell(AOIEntity aoiEntity, int cellX, int cellY, HashSet<long> enterCell, HashSet<long> leaveCell)
    {
        enterCell.Clear();
        leaveCell.Clear();
        int r = (aoiEntity.ViewDistance - 1) / AOIComponent.CellSize + 1;
        int leaveR = r;
        if (aoiEntity.Unit.UnitType == UnitType.Player)
        {
            leaveR += 1;
        }

        for (int i = cellX - leaveR; i <= cellX + leaveR; ++i)
        {
            for (int j = cellY - leaveR; j <= cellY + leaveR; ++j)
            {
                long cellId = CreateCellId(i, j);
                leaveCell.Add(cellId);

                if (i > cellX + r || i < cellX - r || j > cellY + r || j < cellY - r)
                {
                    continue;
                }

                enterCell.Add(cellId);
            }
        }
    }

    public static void AddAOI(Unit unit)
    {
        var aoiComponent = unit.Scene.GetComponent<AOIComponent>();

        if (aoiComponent == null)
        {
            return;
        }

        var aoiEntity = unit.AddComponent<AOIEntity>();
        aoiComponent.Add(unit.Position.x, unit.Position.z, aoiEntity);
    }

    public static void AddAOI(Unit unit, int viewDistance)
    {
        var aoiComponent = unit.Scene.GetComponent<AOIComponent>();

        if (aoiComponent == null)
        {
            return;
        }

        var aoiEntity = unit.AddComponent<AOIEntity>();
        aoiComponent.Add(unit.Position.x, unit.Position.z, aoiEntity);
    }

    public static void RemoveAOI(Unit unit)
    {
        var aoiEntity = unit.GetComponent<AOIEntity>();

        if (aoiEntity == null)
        {
            return;
        }

        unit.Scene.GetComponent<AOIComponent>().Remove(aoiEntity);
        unit.RemoveComponent<AOIEntity>();
    }

    public static long CreateCellId(int x, int y)
    {
        return (long)((ulong)x << 32) | (uint)y;
    }

    public static void SendClientSight(Unit unit)
    {
        var aoiEntity = unit.GetComponent<AOIEntity>();

        // 没有进入AOI 或者 视野中没有Unit
        if (aoiEntity == null || aoiEntity.SeeUnits.Count <= 0)
        {
            return;
        }

        //var m2cUnitCreate = new M2C_UnitCreate();

        foreach (var kv in aoiEntity.SeeUnits)
        {
            var seeAoiEntity = kv.Value;
            var sight = (Unit)seeAoiEntity.Parent;

            if (unit.Id == sight.Id)
            {
                continue;
            }

            // var unitInfo = sight.CreateUnitInfo();
            // m2cUnitCreate.UnitInfos.Add(unitInfo);
        }

        Log.Debug("通知客户端");
        // 通知客户端有新的Unit进入到视野中
        //MessageHelper.SendInnerRoute(unit.Scene, unit.SessionRuntimeId, m2cUnitCreate);
    }

    /// <summary>
    /// 获取范围Units
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="pos"></param>
    /// <param name="dis">米*10000</param>
    public static void GetRangeUnits(Scene scene, Vector3 pos, int dis, ListPool<Unit> units)
    {
        if (dis > 150000)
        {
            Log.Error($"GetRangeUnits is Over {dis}");
            dis = 150000;
        }

        var aoiComponent = scene.GetComponent<AOIComponent>();

        if (aoiComponent == null)
        {
            return;
        }

        var cellX = pos.x.GetCellPos();
        var cellY = pos.z.GetCellPos();

        int r = (dis - 1) / AOIComponent.CellSize + 1;

        var leaveR = r;

        // 计算出需要订阅的格子
        for (int x = cellX - leaveR; x <= cellX + leaveR; x++)
        {
            for (var y = cellY - leaveR; y <= cellY + leaveR; y++)
            {
                var cellId = CreateCellId(x, y);

                if (!aoiComponent.Cells.TryGetValue(cellId, out Cell cell))
                {
                    continue;
                }

                foreach (var aoi in cell.Units.Values)
                {
                    //units.Add(aoi.Unit);
                }
            }
        }
    }

    public static void GetSeePlayers(Unit unit, List<Unit> list)
    {
        var aoi = unit.GetComponent<AOIEntity>();
        if (aoi == null) return;

        var units = aoi.BeSeePlayers;
        foreach (var (_, aoiEntity) in units)
        {
             Unit aoiUnit = aoiEntity.Unit;
             list.Add(aoiUnit);
        }
    }

    // 限制AOI范围为看见的玩家
    public static void AoiLimitAction(Unit unit, Action<Unit> action)
    {
        var aoi = unit.GetComponent<AOIEntity>();
        if (aoi == null) return;

        var units = aoi.BeSeePlayers;

        // 把自己也加进去
        if (!units.ContainsKey(unit.Id))
        {
            units.Add(unit.Id, unit.GetComponent<AOIEntity>());
        }

        foreach (var (_, aoiEntity) in units)
        {
            Unit aoiUnit = aoiEntity.Unit;
            action.Invoke(aoiUnit);
        }
    }
}