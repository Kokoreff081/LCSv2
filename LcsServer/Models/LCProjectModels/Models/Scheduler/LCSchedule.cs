using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.Scheduler;

public class LCSchedule : LCScheduleObject, ISaveLoad
{
    private int _index;
    private bool _isCurrent;//== is playing now
    //private bool _isPlaying;
    [SaveLoad]
    private IEnumerable<object> _saveScheduleItems;
    
    public LCScheduleItem CurrentScheduleItem { get; set; }
    public LCScheduleItem NextScheduleItem { get; set; }


    public LCSchedule(int id, int parentId, string name, int index, bool isCurrent)
    {
        Id = id;
        SaveParentId = parentId;
        Name = name;
        _index = index;
        IsCurrent = isCurrent;
        ScheduleItems = new List<LCScheduleItem>();
    }
    public string LcScheduleName
    {
        get { return Name; }
        set { Name = value; }
    }
    public bool IsCurrent
    {
        get => _isCurrent;
        set
        {
            _isCurrent = value;
        }
    }
    //public bool IsPlaying
    //{
    //    get { return _isPlaying; }
    //    set
    //    {
    //        _isPlaying = value;
    //    }
    //}
    public int Index
    {
        get { return _index; }
        set 
        {
            _index = value;
        }
    }
    public void ChangingName()
    {
        OnNameChanged();
    }
    public List<LCScheduleItem> ScheduleItems { get; set; }

    public void Save(string projectFolderPath)
    {
        _saveScheduleItems = ScheduleItems.Select(s => new { s.Id, s.SelectedWeekDays, s.IsLooped, s.Name, s.ParentId, s.ScenarioId, s.ScenarioNameForRestore, s.SpecifiedDateTime, s.SpecifiedDateTimes, s.TimeType }).ToArray();
    }

    public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
    {
        List<LCScheduleItem> allItems = primitives.OfType<LCScheduleItem>().ToList();

        ScheduleItems = new List<LCScheduleItem>();
        foreach(var item in allItems)
        {
            if (item.ParentId == Id)
                ScheduleItems.Add(item);
        }
    }
}