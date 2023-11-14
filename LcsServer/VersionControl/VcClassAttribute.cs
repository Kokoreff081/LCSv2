namespace LCSVersionControl;

public class VcClassAttribute : Attribute
{
    public string JsonName { get; set; }
    public int Version { get; set; }

    public override string ToString()
    {
        return $"{JsonName}_{Version}";
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is VcClassAttribute otherVcClassAttribute)
        {
            if (otherVcClassAttribute.Version == Version && otherVcClassAttribute.JsonName.Equals(JsonName))
                return true;
        }

        return false;
    }
}