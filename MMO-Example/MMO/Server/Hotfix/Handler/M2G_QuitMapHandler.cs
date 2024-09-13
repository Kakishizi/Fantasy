using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;
using Fantasy.Network.Route;

namespace BestGame;

/// <summary>
/// 玩家从地图菜单退出游戏，从Map发来消息通知网关退出游戏
/// </summary>
public class M2G_QuitMapHandler : Route<Scene,M2G_QuitMapMsg>
{
    protected override async FTask Run(Scene scene,M2G_QuitMapMsg message)
    {
        var gateAccountManager = scene.GetComponent<GateAccountManager>();

        if (gateAccountManager.TryGetValue(message.AccountId, out GateAccount gateAccount))
        {
            var gateRole = gateAccount.GetCurRole();

            // 记录最后进入的地图
            gateRole.LastMap = message.MapNum;
            
            gateAccountManager.QuitGame(gateAccount); 

            await AddressableHelper.RemoveAddressable(scene,gateAccount.AddressableId);
        }
    }
}