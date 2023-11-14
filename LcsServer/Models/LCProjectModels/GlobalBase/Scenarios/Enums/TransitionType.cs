using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

/// <summary>
/// Типы эффектов переходов для сценарии
/// </summary>
public enum TransitionType
{
    /// <summary>
    /// Наложение
    /// </summary>
    [Description(nameof(str.TransitionType_Mixing))]
    Mixing,

    /// <summary>
    /// Плавный переход
    /// </summary>
    [Description(nameof(str.TransitionType_Fading))]
    Fading
}