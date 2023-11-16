using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
namespace LcsServer.Models.LCProjectModels.GlobalBase.Addressing;

public class LCUniverse : IEquatable<LCUniverse>, ILCContainer
{
    private int Id { get; }

    private int _lastAddress;
    private readonly List<LCFootprint> _footprints = new List<LCFootprint>();

    public LCUniverse(int id, int dmxSize)
    {
        Id = id;
        DmxSize = dmxSize;
    }

    public List<LCFootprint> Footprints
    {
        get { return _footprints; }
    }
    public int DmxSize { get; }

    public void Add(int dmxAddress, int footprint, int colorIndex)
    {
        var lcFootprint = new LCFootprint(dmxAddress, footprint, colorIndex);
        _footprints.Add(lcFootprint);
        // подсчитываем последний значимый байт
        int last = dmxAddress + footprint;
        if (last > _lastAddress)
        {
            _lastAddress = last;
        }
    }

    /// <summary>
    /// Подготовка буфера юниверса для отправки на устройство
    /// </summary>
    /// <param name="source">массив цветов</param>
    /// <param name="insertFirstZeroByte">для ILCS надо вставить первый нулевой байт</param>
    /// <param name="fullSizePacket">отправлять полный пакет (DMXSize) или ограничить ненулевыми данными</param>
    /// <returns></returns>
    public byte[] GetBuffer(byte[] source, bool insertFirstZeroByte, bool fullSizePacket = true)
    {
        int arraySize = fullSizePacket ? DmxSize : _lastAddress;
        byte[] buffer = insertFirstZeroByte ? new byte[arraySize + 1] : new byte[arraySize];
        _footprints.ForEach(x => x.PlaceData(buffer, source, insertFirstZeroByte));
        return buffer;
    }

    public override bool Equals(object other)
    {
        return Equals(other as LCUniverse);
    }

    public bool Equals(LCUniverse other)
    {
        bool result = other != null && Id == other.Id && _footprints.Count == other._footprints.Count;

        if (!result)
            return false;

        for (int i = 0; i < _footprints.Count; i++)
        {
            if (_footprints[i] != other._footprints[i])
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        int result = Id;
        foreach (var footprint in _footprints)
        {
            result = result * 31 ^ footprint.GetHashCode();
        }

        return result;
    }

}
