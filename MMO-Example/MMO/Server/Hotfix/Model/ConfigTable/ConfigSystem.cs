using cfg;
using Fantasy;
using Fantasy.Entitas.Interface;
using Fantasy.Platform.Net;
using Luban;

namespace Hotfix;

public class ConfigSystem
{
    private static ConfigSystem _instance = null;

    public static ConfigSystem Instance
    {
        get { return _instance ??= new ConfigSystem(); }
        private set => _instance = value;
    }

    private Tables _tables;
    private ConfigSystem()
    {
        _tables = new Tables(file =>
            new ByteBuf(File.ReadAllBytes(Path.Combine(ProcessDefine.ConfigTableBinaryDirectory + "/Luban",
                $"{file}.bytes"))));
        Log.Debug("== load succ ==");
    }
    
    public UnitConfig GetUnitConfig(int id)
    {
        return _tables.TbUnitConfig.Get(id);
    }
    
    public List<UnitConfig> GetUnitConfigList()
    {
        return _tables.TbUnitConfig.DataList;
    }
}