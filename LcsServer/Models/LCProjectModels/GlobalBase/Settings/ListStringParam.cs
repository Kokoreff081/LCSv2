using System.Xml;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Settings;

public class ListStringParam : UniParam
{
    public new List<string> Value
    {
        get { return (List<string>)base.Value; }
        set { base.Value = value; }
    }
    public ListStringParam(string name, string saveName, ControlParamEnum type = ControlParamEnum.None) : base(name, saveName, type)
    {
        Value = new List<string>();
    }

    public static implicit operator List<string>(ListStringParam param)
    {
        return param.Value;
    }

    public int LimitCount { get; set; }

    public void AddToStart(string str)
    {
        Value.Insert(0, str);
        Distinct();
        if (LimitCount > 0 && Value.Count > LimitCount)
        {
            Value.RemoveAt(Value.Count - 1);
        }
    }

    public void Remove(string str)
    {
        //if (Contains(str))
        Value.Remove(str);
    }

    public bool Any()
    {
        return Value.Any();
    }

    public bool Contains(string str)
    {
        return !string.IsNullOrWhiteSpace(str) && Value.Contains(str);
    }

    public override void ReadAttributes(XmlAttributeCollection attributes)
    {
        Value = new List<string>();
        foreach (XmlAttribute attribute in attributes)
        {
            Value.Add(attribute.Value);
        }
        Distinct();
    }

    private void Distinct()
    {
        Value = Value.Distinct().ToList();
    }

    public override void WriteAttributes(XmlElement el)
    {
        if (Value == null || !Value.Any())
        {
            return;
        }

        int k = 1;
        foreach (string a in Value)
        {
            el.SetAttribute($"V_{k++}", a);
        }
    }
}