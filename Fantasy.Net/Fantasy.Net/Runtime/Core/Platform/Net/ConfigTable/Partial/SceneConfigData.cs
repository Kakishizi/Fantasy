#if FANTASY_NET
using Fantasy.DataStructure.Collection;
using Fantasy.IdFactory;
using ProtoBuf;
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Fantasy.Platform.Net;

public sealed partial class SceneConfig
{
    [ProtoIgnore]
    public long RouteId { get; private set; }
    public override void EndInit()
    {
        RouteId = new RuntimeIdStruct(0, Id, (byte)WorldConfigId, 0);
        base.EndInit();
    }
}

public sealed partial class SceneConfigData
{
    [ProtoIgnore]
    private readonly OneToManyList<int, SceneConfig> _sceneConfigBySceneType = new OneToManyList<int, SceneConfig>();
    [ProtoIgnore]
    private readonly OneToManyList<uint, SceneConfig> _sceneConfigByProcess = new OneToManyList<uint, SceneConfig>();
    
    public override void EndInit()
    {
        _sceneConfigByProcess.Clear();
        
        foreach (var sceneConfig in List)
        {
            _sceneConfigByProcess.Add(sceneConfig.ProcessConfigId, sceneConfig);
            _sceneConfigBySceneType.Add(sceneConfig.SceneType, sceneConfig);
        }
    }

    public List<SceneConfig> GetByProcess(uint serverConfigId)
    {
        return _sceneConfigByProcess.TryGetValue(serverConfigId, out var list) ? list : new List<SceneConfig>();
    }

    public List<SceneConfig> GetSceneBySceneType(int sceneType)
    {
        return !_sceneConfigBySceneType.TryGetValue(sceneType, out var list) ? new List<SceneConfig>() : list;
    }
}
#endif