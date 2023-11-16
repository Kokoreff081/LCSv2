using LcsServer.Models.LCProjectModels.GlobalBase;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

public interface IClone
{
    /// <summary>
    /// Клонирование
    /// </summary>
    /// <param name="lcObjectsManager">Ссылка на менеджер объектов LCObjectsManager (для присвоения ID)</param>
    /// <param name="parent">Родитель</param>
    LCObject Clone(BaseLCObjectsManager lcObjectsManager, LCObject parent);
}