namespace LcsServer.Models.ProjectModels;

public class ScenarioNameId
{
    public int ScenarioId { get; set; }
    public string ScenarioName { get; set; }

    public float TotalTicks { get; set; }
    public float ElapsedTicks { get; set; }
    public string? ScenarioTime { get
        {
            if(TotalTicks == null)
                return string.Empty;
            return TimeSpan.FromSeconds((double)TotalTicks).ToString(@"hh\:mm\:ss");
        } 
    }
    public bool IsPlaying { get; set; }
}