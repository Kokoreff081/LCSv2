using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.Project;

public class Project : LCObject, ISaveLoad
{
    public Project()
    {
        ScenarioNames = new List<string>();
        ProjectInfo = new ProjectInfo();
    }

    public Project(int id, string name, Guid projectId, string[] scenarioNames, ProjectInfo projectInfo)
    {
        Id = id;
        Name = name;
        ProjectId = projectId;
        ScenarioNames = new List<string>(scenarioNames);
        ProjectInfo = projectInfo;
    }
    public List<string> ScenarioNames { get; set; }
    public ProjectInfo ProjectInfo { get; set; }
    public string Path { get; set; }
    #region ISave/Load

    /// <summary>
    /// Id проекта
    /// </summary>
    [SaveLoad]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Вызывается при сохранении проекта
    /// </summary>
    /// <param name="projectFolderPath">Путь к папке проекта</param>
    public void Save(string projectFolderPath)
    {
    }

    /// <summary>
    /// Вызывается при загрузке проекта
    /// </summary>
    /// <param name="primitives">Все объекты проекта</param>
    /// <param name="indexInPrimitives">Индекс объекта в списке всех примитивов. Для ускоренного поиска родителя</param>
    /// <param name="projectFolderPath">Путь к папке проекта</param>
    public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
    {
    }

    #endregion
}