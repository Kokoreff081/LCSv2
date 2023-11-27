using System.Xml;
using LcsServer.Models.LCProjectModels.GlobalBase.Scheduler;
using LcsServer.Models.LCProjectModels.GlobalBase.Settings;

namespace LcsServer.Models.LCProjectModels.GlobalBase;

public abstract class BaseSetting : IDisposable
{
    protected const string RootName = "Root";

    public event EventHandler SettingChanged;
    
    protected BaseSetting()
    {
        Location = SettingsLocation.Application;
        List = new List<UniParam>();
    }

    public SettingsLocation Location { get; protected set; }

    public virtual bool IsChanged { get; set; }

    public List<UniParam> List { get; set; }

    public abstract void ToDefault();
    
    public virtual void Load(XmlDocument doc)
    {
        ToDefault();
        var rootNode = doc.SelectSingleNode(RootName);
        if(rootNode != null && rootNode.HasChildNodes)
        {

            foreach(XmlElement childNode in rootNode.ChildNodes)
            {
                var param = List.FirstOrDefault(a => a.PropertyName == childNode.Name);
                param?.ReadAttributes(childNode.Attributes);
            }
        }

        IsChanged = false;
    }

    internal XmlDocument Save()
    {
        IsChanged = false;
        return GetXmlDocument();
    }

    protected virtual XmlDocument GetXmlDocument()
    {
        XmlDocument doc = new XmlDocument();
        var root = doc.CreateElement(RootName);
        doc.AppendChild(root);

        foreach(UniParam param in List)
        {
            XmlElement el = doc.CreateElement(param.PropertyName);
            param.WriteAttributes(el);
            root.AppendChild(el);
        }

        return doc;
    }

    protected void SubscriptionСhanges()
    {
        foreach (UniParam param in List)
        {
            param.Changed += OnChange;
        }
    }

    protected void OnChange(object sender, string[] propertyname)
    {
        OnContainerChanged();
    }

    private void OnContainerChanged()
    {
        SettingChanged?.Invoke(this, EventArgs.Empty);
        IsChanged = true;
    }

    public virtual void Dispose()
    {
        List.ForEach(param => param.Changed -= OnChange);
    }
}