public class MapScene : BaseScene
{
    public override void EnterScene()
    {
        base.EnterScene();
        // 添加UIPanel
        mUIFacade.AddPanelToDict(StringManager.MapUIFramePanel);
        mUIFacade.AddPanelToDict(StringManager.SettingPanel);
        mUIFacade.InitUIPanelDict();
        

        // 打开MapUIFramePanel
        var mapUIFramePanel = mUIFacade.GetUIPanel(StringManager.MapUIFramePanel);
        mapUIFramePanel.EnterPanel();
        mUIFacade.lastPanel = mapUIFramePanel;

        // playerUnits加入场景
        GameApp.PlayerUnits.EnterScene();
        // NpcUnits加入场景
        GameApp.NpcUnits.EnterScene();
        // MonsterUnits加入场景
        GameApp.MonsterUnits.EnterScene();
    }

    public override void ExitScene()
    {
        base.ExitScene();

        // playerUnits退出场景
        GameApp.PlayerUnits.ExitScene();
        // NpcUnits退出场景
        GameApp.NpcUnits.ExitScene();
        // MonsterUnits退出场景
        GameApp.MonsterUnits.ExitScene();
    }

}
