namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public class LCAddressRegisteredMap
{
    private List<LCAddressRegisteredLamp> _registeredLamps;

    /// <summary>
    /// Общее кол-во footprint для всех светильников
    /// </summary>
    public int ByteArraySize { get; private set; }

    public Dictionary<int, LCAddressRegisteredLamp> RegisteredLampsMap { get; private set; }

    public List<LCAddressRegisteredLamp> RegisteredLamps
    {
        get => _registeredLamps;
        set
        {
            _registeredLamps = value;
            RegisteredLampsMap = _registeredLamps.ToDictionary(x => x.LampId, x => x);
            if (_registeredLamps.Count > 0)
                ByteArraySize = _registeredLamps.Sum(x => x.ArraySize);
            else
                ByteArraySize = 0;
        }
    }

    public void Clear()
    {
        RegisteredLampsMap?.Clear();
        _registeredLamps?.Clear();

        ByteArraySize = 0;
    }
}