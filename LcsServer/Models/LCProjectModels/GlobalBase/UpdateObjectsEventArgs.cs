namespace LcsServer.Models.LCProjectModels.GlobalBase;

public enum InformAction
{
    /// <summary>
    /// Добавление
    /// </summary>
    Add,
    /// <summary>
    /// Удаление
    /// </summary>
    Remove,
    /// <summary>
    /// Удалить все
    /// </summary>
    RemoveAll
}
public class UpdateObjectsEventArgs : EventArgs
{
    /// <summary>
    /// Объекты которые были обновлены
    /// </summary>
    public List<LCObject> Objects { get; }
    /// <summary>
    /// Тип действия
    /// </summary>
    public InformAction Action { get; }

    public UpdateObjectsEventArgs(List<LCObject> objects, InformAction action)
    {
        Objects = objects;
        Action = action;
    }
    
}