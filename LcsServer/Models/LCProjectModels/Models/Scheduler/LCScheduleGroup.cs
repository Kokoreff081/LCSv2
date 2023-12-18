using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Scheduler.Enums;

namespace LcsServer.Models.LCProjectModels.Models.Scheduler;

public class LCScheduleGroup : LCScheduleObject, ISaveLoad
{
    private int _index;
    private int _dimmingLevel;
    private bool _isCurrent;
    private string _description;
    private bool _isAutoStart;

    public event EventHandler DescriptionChanged;

    public LCScheduleGroup(int id, int parentId, string name, int dimmingLevel, int index, bool isCurrent, string description = "")
    {
        Id = id;
        SaveParentId = parentId;
        Name = name;
        DimmingLevel = dimmingLevel;
        Index = index;
        IsCurrent = isCurrent;
        Schedules = new List<LCSchedule>();
        _description = description;
    }

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
            OnDescriptionChanged();
        }
    }

    public bool IsAutoStart
    {
        get { return _isAutoStart; }
        set { _isAutoStart = value; }
    }
    public List<LCSchedule> Schedules { get; set; }

    private void OnDescriptionChanged()
    {
        DescriptionChanged?.Invoke(this, EventArgs.Empty);
    }
    public List<LCScheduleObject> GetGroupObjects()
    {
        var lst = new List<LCScheduleObject>();
        lst.AddRange(Schedules);
        foreach(var schedule in Schedules)
        {
            foreach (var schedItem in schedule.ScheduleItems)
                schedItem.SaveParentId = schedule.Id;
        
            lst.AddRange(schedule.ScheduleItems);
        }
        return lst;
    }
    public void Load(List<ISaveLoad> primitives, int indexInPrimitives)
    {
       
    }

    public void Save(string projectFolderPath)
    {
        
    }
}
