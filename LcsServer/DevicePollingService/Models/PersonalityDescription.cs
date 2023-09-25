namespace LcsServer.DevicePollingService.Models;

public class PersonalityDescription : BaseObject
{
    private short _dmxSlotsRequired;
    private string _description;

    public short DmxSlotsRequired
    {
        get => _dmxSlotsRequired;
        set => _dmxSlotsRequired = value;
    }

    public byte PersonalityIndex { get; }

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    public override string ToString()
    {
        return Description;
    }

    public PersonalityDescription(byte personalityIndex, string parentId, short dmxSlotsRequired, string description) : base(personalityIndex.ToString(), parentId)
    {
        PersonalityIndex = personalityIndex;
        DmxSlotsRequired = dmxSlotsRequired;
        Description = description;
    }
}