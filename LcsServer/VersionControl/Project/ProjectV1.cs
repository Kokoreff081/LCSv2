using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Project;
using LCSVersionControl.Converters;
using Newtonsoft.Json;

namespace LCSVersionControl.Project;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class ProjectV1 : BaseVC
{
    private const string ModelClassName = "Project";

    public Guid ProjectId { get; set; }

    public string[] Scenarios { get; set; }

    #region ProjectInfo

    public string Title { get; set; }

    public string Date { get; set; }

    public string AssembledBy { get; set; }

    public string Customer { get; set; }

    public string Description { get; set; }

    public string ObjectAddress { get; set; }
    
    public string ObjectCoordinates { get; set; }

    #endregion

    public override ISaveLoad ToConcreteObject()
    {
         ProjectInfo projectInfo = new ProjectInfo
        {
            Date = Convert.ToDateTime(Date, Helper.Culture),
            AssembledBy = new CompanyInfo(AssembledBy),
            Customer = new CompanyInfo(Customer),
            Description = Description,
            ObjectAddress = ObjectAddress,
            ObjectCoordinates = ObjectCoordinates,
            Title = Title
        };

        LcsServer.Models.LCProjectModels.Models.Project.Project project = new LcsServer.Models.LCProjectModels.Models.Project.Project(Id, Name, ProjectId, Scenarios, projectInfo);

        return project;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.Project.Project project)
        {
            return null;
        }

        ProjectV1 projectVc = new ProjectV1
        {
            Id = project.Id,
            Name = project.Name,
            ProjectId = project.ProjectId,
            Scenarios = project.ScenarioNames.ToArray(),

            Title = project.ProjectInfo.Title,
            Date = project.ProjectInfo.Date.ToString(Helper.Culture),
            AssembledBy = project.ProjectInfo.AssembledBy == null
                ? string.Empty
                : project.ProjectInfo.AssembledBy.ToString(),
            Customer =
                project.ProjectInfo.Customer == null ? string.Empty : project.ProjectInfo.Customer.ToString(),
            Description = project.ProjectInfo.Description ?? string.Empty,
            ObjectAddress = project.ProjectInfo.ObjectAddress ?? string.Empty,
            ObjectCoordinates = project.ProjectInfo.ObjectCoordinates ?? string.Empty
        };

        return projectVc;

    }
}