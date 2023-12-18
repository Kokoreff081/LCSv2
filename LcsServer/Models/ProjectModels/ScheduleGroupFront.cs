using LcsServer.Models.LCProjectModels.Models.Scheduler;

namespace LcsServer.Models.ProjectModels;

public class ScheduleGroupFront
{
    private int _index;
    private int _dimmingLevel;
    private bool _isCurrent;
    private string _description;
    private bool _isAutoStart;
    private int _id;
    private string _name;
    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }
    public int DimmingLevel
    {
        get { return _dimmingLevel; }
        set { _dimmingLevel = value; }
    }
    public bool IsCurrent
    {
        get { return _isCurrent; }
        set { _isCurrent = value; }
    }

    public string Description
    {
        get { return _description; }
        set 
        {
            _description = value;
            //OnDescriptionChanged();
        }
    }

    public bool IsAutoStart
    {
        get { return _isAutoStart; }
        set { _isAutoStart = value; }
    }

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public List<LcScheduleFront> Schedules { get; set; }
    
    public LcScheduleFront? SelectedSchedule { get; set; }
    public LcScheduleFront? PlayingSchedule { get; set; }
}