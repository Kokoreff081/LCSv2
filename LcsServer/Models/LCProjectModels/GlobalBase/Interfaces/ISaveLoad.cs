namespace LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

public interface ISaveLoad
{
    /// <summary>
    /// Вызывается при сохранении проекта
    /// </summary>
    /// <param name="projectFolderPath">Путь к папке проекта</param>
    void Save(string projectFolderPath);

    /// <summary>
    /// Вызывается при загрузке проекта
    /// </summary>
    /// <param name="primitives">Все объекты проекта</param>
    /// <param name="indexInPrimitives">Индекс объекта в списке всех примитивов. Для ускоренного поиска родителя</param>
    /// <param name="projectFolderPath">Путь к папке проекта</param>
    void Load(List<ISaveLoad> primitives, int indexInPrimitives);
}