using LcsServer.Models.LCProjectModels.Models.Scheduler;

namespace LcsServer.Models.ProjectModels;

public class LcScheduleFront
{
    private int _index;
    private bool _isCurrent;//== is playing now
    private bool _isSelected;//private bool _isPlaying;
    public LCScheduleItem CurrentScheduleItem { get; set; }
    public LCScheduleItem NextScheduleItem { get; set; }
    
    public bool IsCurrent
    {
        get => _isCurrent;
        set
        {
            _isCurrent = value;
        }
    }
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
        }
    }
    public int Index
    {
        get { return _index; }
        set 
        {
            _index = value;
        }
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ScheduleItemFront> ScheduleItems { get; set; }
}