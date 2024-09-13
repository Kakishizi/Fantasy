using Fantasy;
using Fantasy.Helper;
using Fantasy.Platform.Net;

namespace BestGame;

public static class SceneHelper
{
    // 根据routeId获取Scene的EntityId
    public static long GetSceneEntityId(uint routeId)
    {
        var enityId = 0L;
        
        foreach (var sceneConfig in SceneConfigData.Instance.List)
        {
            if (sceneConfig.RouteId == routeId)
            {
                enityId = sceneConfig.RouteId;
                break;
            }
        }
        return enityId;
    }

    // 获取Scene的外网地址
    public static string GetOutAddress(uint sceneId)
    {
        var sceneConfig = SceneConfigData.Instance.Get(sceneId);
        return GetOutAddressByServer(sceneConfig.ProcessConfigId);
    }

    // 获取Server的外网地址
    public static string GetOutAddressByServer(uint ProcessId)
    {
        foreach (var serverConfig in ProcessConfigData.Instance.List)
        {
            if (serverConfig.Id == ProcessId)
            {
                return MachineConfigData.Instance.Get(serverConfig.MachineId).OuterIP;
            }
        }

        // 处理未找到服务器的情况，返回默认值或抛出异常，具体取决于需求
        return string.Empty;
    }

    // 根据区服随机获取一个指定类型服务器配置
    public static SceneConfig GetSceneRandom(int type,uint world)
    {
        var sceneList = GetSceneByWorld(world,type); 
        var i = RandomHelper.RandomNumber(0, sceneList.Count);
        return sceneList[i];
    }

    // 根据区服world获取服务器Scene配置列表(可约束类型)
    public static List<SceneConfig> GetSceneByWorld(uint world, int type = 0)
    {
        return SceneConfigData.Instance.List
            .Where(sceneConfig => sceneConfig.WorldConfigId == world 
                && (type == 0 || sceneConfig.SceneType ==type ))
            .ToList();
    }
}