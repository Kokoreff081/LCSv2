using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

/// <summary>
/// Вращение
/// </summary>
public enum Rotation
{
    /// <summary>
    /// По часовой
    /// </summary>
    [Description(nameof(str.Rotation_Clockwise))]
    Clockwise = 0,

    /// <summary>
    /// Против часовой
    /// </summary>
    [Description(nameof(str.Rotation_CounterClockwise))]
    CounterClockwise = 1,

    /// <summary>
    /// Случайно
    /// </summary>
    [Description(nameof(str.Rotation_Random))]
    Random = 2
}