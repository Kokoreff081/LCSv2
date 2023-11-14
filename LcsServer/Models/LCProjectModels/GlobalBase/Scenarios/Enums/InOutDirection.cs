using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

/// <summary>
/// Направление
/// </summary>
public enum InOutDirection
{
    /// <summary>
    /// Внутрь
    /// </summary>
    [Description(nameof(str.InOutDirection_Inside))]
    Inside = 0,

    /// <summary>
    /// Наружу
    /// </summary>
    [Description(nameof(str.InOutDirection_OutSide))]
    Outside = 1,

    /// <summary>
    /// Случайно
    /// </summary>
    [Description(nameof(str.InOutDirection_Random))]
    Random = 2
}