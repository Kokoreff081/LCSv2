using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

/// <summary>
/// Тип заполнения
/// </summary>
public enum FillingTypes
{
    /// <summary>
    /// Статичный цвет
    /// </summary>
    [Description(nameof(str.EffectFillingTypes_Static))]
    Static,

    /// <summary>
    /// Переход от одного цвета к другому
    /// </summary>
    [Description(nameof(str.EffectFillingTypes_Transition))]
    Transition,

    /// <summary>
    /// Заполняющий
    /// </summary>
    [Description(nameof(str.EffectFillingTypes_Fill))]
    Fill,

    /// <summary>
    /// Вспышка
    /// </summary>
    [Description(nameof(str.EffectFillingTypes_Flash))]
    Flash,

    /// <summary>
    /// Градиент
    /// </summary>
    [Description(nameof(str.EffectFillingTypes_Gradient))]
    Gradient,
}