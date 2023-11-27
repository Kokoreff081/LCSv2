using System.Xml;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Settings;

public class StringParam : UniParam
{
    public new string Value
    {
        get { return (string)base.Value; }
        set { base.Value = value; }
    }

    public StringParam(string name, string saveName, ControlParamEnum type = ControlParamEnum.TextEdit) : base(name, saveName, type)
    {
        Value = string.Empty;
    }

    public static implicit operator string(StringParam param)
    {
        return param?.Value ?? string.Empty;
    }

    public override void ReadAttributes(XmlAttributeCollection attributes)
    {
        foreach(XmlAttribute attribute in attributes)
        {
            switch(attribute.Name)
            {
                case "Value":
                    Value = attribute.Value;
                    break;
            }
        }
    }

    public override void WriteAttributes(XmlElement el)
    {
        el.SetAttribute("Value", Value);
    }

    public override string ToString()
    {
        return Value;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}