using System.Collections.Concurrent;
using LcsServer.DevicePollingService.Interfaces;

namespace LcsServer.DevicePollingService.Models;

public class ParametersSettings : BaseSettings
{
    private readonly ConcurrentDictionary<string, string> _parametersName;

    public ParametersSettings()
    {
        _parametersName = new ConcurrentDictionary<string, string>();
    }

    public IReadOnlyDictionary<string, string> ParametersName => _parametersName;

    public void AddParameter(string parameter, string name)
    {
        _parametersName[parameter] = name;
        IsChanged = true;
    }

    public override void Load(ISerializationManagerRDM serializationManager, byte[] data)
    {
        ParametersSettings parametersSettings = serializationManager.Deserialize<ParametersSettings>(data);

        _parametersName.Clear();

        foreach (var parameter in parametersSettings.ParametersName)
        {
            _parametersName.AddOrUpdate(parameter.Key, parameter.Value, (key, value) => parameter.Value);
        }
    }
}