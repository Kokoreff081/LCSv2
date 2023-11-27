using System.Xml;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Settings;

public class BoolParam : UniParam
{
    public new bool Value
    {
        get { return (bool)base.Value; }
        set { base.Value = value; }
    }

    public BoolParam(string name, string saveName, ControlParamEnum type = ControlParamEnum.CheckEdit) : base(name, saveName, type) { }

    public static implicit operator bool(BoolParam param)
    {
        return param?.Value ?? false;
    }

    public override void ReadAttributes(XmlAttributeCollection attributes)
    {
        foreach(XmlAttribute attribute in attributes)
        {
            switch(attribute.Name)
            {
                case "Value":
                    if(attribute.Value == "0")
                    {
                        Value = false;
                    }
                    else if(attribute.Value == "1")
                    {
                        Value = true;
                    }
                    break;
            }
        }
    }

    public override void WriteAttributes(XmlElement el)
    {
        el.SetAttribute("Value", Value ? "1" : "0");
    }
}