using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;

namespace LLcsServer.DevicePollingService.Models;

public class PersonalitySlotInformation : BaseObject
{
    public short Offset { get; set; }

    public SlotTypes Type { get; set; }

    public SlotIds SlotId { get; set; }

    public int SlotLink { get; set; }

    public string Description { get; set; }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Description))
            return Description;

        if (Type == SlotTypes.Primary || Type == SlotTypes.Undefined)
            return SlotId.ToString();
        return Type.ToString();
    }

    public PersonalitySlotInformation(SlotIds slotId, string parentId) : base(slotId.ToString(), parentId)
    {
    }
}