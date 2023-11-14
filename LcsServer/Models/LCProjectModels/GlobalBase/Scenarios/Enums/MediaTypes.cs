using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

/// <summary>
/// Тип медиа
/// </summary>
public enum MediaTypes
{
    /// <summary>
    /// Видео
    /// </summary>
    [Description(nameof(str.EffectMediaTypes_Avi))]
    Avi,

    /// <summary>
    /// Картинка
    /// </summary>
    [Description(nameof(str.EffectMediaTypes_Texture))]
    Texture
}