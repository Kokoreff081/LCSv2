using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

/// <summary>
/// Природные типы
/// </summary>
public enum NatureTypes
{
    /// <summary>
    /// Пламя
    /// </summary>
    [Description(nameof(str.EffectNatureTypes_Flame))]
    Flame,
        
    /// <summary>
    /// Звезды
    /// </summary>
    [Description(nameof(str.EffectParticleTypes_Stars))]
    Stars
}