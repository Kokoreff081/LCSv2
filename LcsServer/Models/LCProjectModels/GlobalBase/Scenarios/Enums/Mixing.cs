using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public enum Mixing
{
    /// <summary>
    /// Нормальное
    /// </summary>
    [Description(nameof(str.Mixing_Normal))]
    Normal,

    /// <summary>
    /// Растворение
    /// </summary>
    [Description(nameof(str.Mixing_Dissolve))]
    Dissolve,

    /// <summary>
    /// Затемнение
    /// </summary>
    [Description(nameof(str.Mixing_Darken))]
    Darken,

    /// <summary>
    /// Многообразие
    /// </summary>
    [Description(nameof(str.Mixing_Multiply))]
    Multiply,
    //ColorBurn,
    //LinearBurn,
    //DarkerColor,

    /// <summary>
    /// Осветление
    /// </summary>
    [Description(nameof(str.Mixing_Lighten))]
    Lighten,
    /// <summary>
    /// При смешивании использовать последний полученый цвет
    /// </summary>
    [Description(nameof(str.Mixing_LastReceived))]
    LastReceived,

    //Screen,
    //ColorDodge,
    //LinearDodge,
    //LighterColor,
    //Overlay,
    //SoftLight,
    //HardLight,
    //VividLight,
    //LinearLight,
    //PinLight,
    //HardMix,
    //Difference,
    //Exclusion,
    //Saturation,
    //Color,
    //Luminosity,
}