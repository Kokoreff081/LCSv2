namespace LcsServer.Models.LCProjectModels.GlobalBase;

public abstract class LCObject
{
    private string _name = string.Empty;

    /// <summary>
    /// Название измененно
    /// </summary>
    public event EventHandler NameChanged;
           
    /// <summary>
    /// Id объекта
    /// </summary>
    [SaveLoad]
    public int Id { get; set; }

    /// <summary>
    /// Id родителя, если его нет, то возвращается 0
    /// </summary>
    [SaveLoad]
    public virtual int ParentId => 0;

    /// <summary>
    /// Название объекта
    /// </summary>
    [SaveLoad]
    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;

            _name = value;
            OnNameChanged();
        }
    }

    /// <summary>
    /// Название для отображения
    /// </summary>
    public virtual string DisplayName => _name;

    /// <summary>
    /// Вызывает собитие Название измененно
    /// </summary>
    protected void OnNameChanged()
    {
        NameChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Возвращает всех детей объекта до последнего уровня, если детей нет, то возвращает пустой список
    /// </summary>
    /// <returns>Список всех детей</returns>
    public virtual List<T> GetAllChildren<T>() where T: LCObject
    {
        return new List<T>();
    }

    public List<T> GetAllLCObjects<T>() where T : LCObject
    {
        List<T> result = GetAllChildren<T>();
        if (this is T t)
        {
            result.Insert(0, t);
        }

        return result;
    }

}