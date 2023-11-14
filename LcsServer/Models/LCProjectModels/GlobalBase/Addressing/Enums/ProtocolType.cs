using System.ComponentModel;
using LightCAD.UI.Strings;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;

public enum ProtocolType
{
    [Description(nameof(Resources.ArtNet))]
    ArtNet,
    [Description("sACN")]
    SAcn
}