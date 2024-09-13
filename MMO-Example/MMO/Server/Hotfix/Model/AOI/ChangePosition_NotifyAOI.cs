using Fantasy.Event;
using UnityEngine;

namespace Fantasy;

public class ChangePosition_NotifyAOI : EventSystem<ChangePostion>
{
    protected override void Handler(ChangePostion args)
    {
        Unit unit = args.unit;
        Vector3 oldPos = args.oldPos;
        int oldCellX = (int)(oldPos.x * 1000) / AOIComponent.CellSize;
        int oldCellY = (int)(oldPos.z * 1000) / AOIComponent.CellSize;
        int newCellX = (int)(unit.Position.x * 1000) / AOIComponent.CellSize;
        int newCellY = (int)(unit.Position.z * 1000) / AOIComponent.CellSize;
        if (oldCellX == newCellX && oldCellY == newCellY)
        {
            return;
        }

        AOIEntity aoiEntity = unit.GetComponent<AOIEntity>();
        if (aoiEntity == null)
        {
            return;
        }
        unit.Scene.GetComponent<AOIComponent>().Move(aoiEntity, newCellX, newCellY);
    }
}