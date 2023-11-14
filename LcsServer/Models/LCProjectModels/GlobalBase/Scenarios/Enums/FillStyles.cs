using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;


namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

/// <summary>
/// Заполнение
/// </summary>
public enum FillStyles
{
    /// <summary>
    /// Из одной стороны 
    /// </summary>
    [Description(nameof(str.FillStyles_OneSide))]
    OneSide = 0,

    /// <summary>
    /// С двух сторон
    /// </summary>
    [Description(nameof(str.FillStyles_TwoSides))]
    TwoSides = 1,

    /// <summary>
    /// Прямоугольником
    /// </summary>
    [Description(nameof(str.FillStyles_Rect))]
    Rect = 2,

    /// <summary>
    /// Ромбом
    /// </summary>
    [Description(nameof(str.FillStyles_Rhombus))]
    Rhombus = 3,

    /// <summary>
    /// Кругом
    /// </summary>
    [Description(nameof(str.FillStyles_Circle))]
    Circle = 4,

    /// <summary>
    /// Веером
    /// </summary>
    [Description(nameof(str.FillStyles_Fan))]
    Fan = 5
}