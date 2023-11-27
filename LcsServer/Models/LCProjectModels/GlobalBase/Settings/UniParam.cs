using System.Xml;
using NLog;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Settings;

public delegate void StringDelegate(object sender, params string[] propertyName);

public class UniParam
{
    protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public string PropertyName => _saveName;

    public event StringDelegate Changed;

    private object _val;
    public virtual object Value
    {
        get { return _val; }
        set
        {
            if (_val == null && value == null)
                return;

            if (_val == null || !_val.Equals(value))
            {
                _val = ValidateValue(value);
                OnChanged(nameof(Value));
            }
        }
    }

    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            if(_name != value)
            {   
                _name = value;
                OnChanged(nameof(Name));
            }
        }
    }
    
    private readonly string _saveName;

    public ControlParamEnum Type { get; private set; }

    public UniParam(string name, string propName, ControlParamEnum type)
    {
        _saveName = propName;
        _name = name;
        Type = type;
    }
    
    public virtual void ReadAttributes(XmlAttributeCollection attributes)
    {
        foreach(XmlAttribute attribute in attributes)
        {
            switch(attribute.Name)
            {
                case "Value":
                    Value = _val;
                    break;
            }
        }
    }

    public virtual void WriteAttributes(XmlElement el)
    {
        el.SetAttribute("Value", Value.ToString());
    }

    protected virtual void OnChanged(params string[] propertyName)
    {
        Changed?.Invoke(this, propertyName);
    }
    
    protected virtual object ValidateValue(object value)
    {
        return value;
    }
}
public enum ControlParamEnum : byte
{
    None,
    TextEdit,
    CheckEdit,
    ComboBoxEdit,
    TextEditorWithMask,
    PopupColorEdit,
    Slider,
    ExpSlider,
    ComboBoxEx,
}