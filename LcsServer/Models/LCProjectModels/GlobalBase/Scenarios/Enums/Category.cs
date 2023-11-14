using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;
namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

/// <summary>
/// Категория эффектов
/// </summary>
public enum Category
{
    /// <summary>
    /// Заполняющие
    /// </summary>
    [Description(nameof(str.EffectType_Filling))]
    Filling,

    /// <summary>
    /// Природные
    /// </summary>
    [Description(nameof(str.EffectType_Nature))]
    Nature,

    /// <summary>
    /// Частицы
    /// </summary>
    [Description(nameof(str.EffectType_Particle))]
    Particle,

    /// <summary>
    /// Медиа
    /// </summary>
    [Description(nameof(str.EffectType_Media))]
    Media,
        
    [Description(nameof(str.EffectType_Sound))]
    Sound,
        
    [Description(nameof(str.EffectType_Modbus))]
    Modbus
}