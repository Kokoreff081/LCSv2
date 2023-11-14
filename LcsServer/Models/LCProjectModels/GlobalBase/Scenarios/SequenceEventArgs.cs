namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public class SequenceEventArgs : EventArgs
{
    public SequenceEventArgs(bool isAddItems, bool isRemoveItems, bool isChangeStarts)
    {
        IsAdding = isAddItems;
        IsRemoving = isRemoveItems;
        IsChangeStarts = isChangeStarts;
    }

    public bool IsAdding { get; }
    public bool IsRemoving { get; }
    public bool IsChangeStarts { get; }

    public bool IsChangeStartsOnly => !IsAdding && !IsRemoving && IsChangeStarts;
}