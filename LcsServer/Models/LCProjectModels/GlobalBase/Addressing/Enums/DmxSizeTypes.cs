using System.ComponentModel;
using LightCAD.UI.Strings;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;

public enum DmxSizeTypes
{
    [Description(nameof(Resources.Dmx512))]
    Dmx512 = 512,

    [Description(nameof(Resources.Dmx1024))]
    Dmx1024 = 1024,

    [Description(nameof(Resources.Dmx2048))]
    Dmx2048 = 2048,

    [Description(nameof(Resources.Dmx4096))]
    Dmx4096 = 4096,
}