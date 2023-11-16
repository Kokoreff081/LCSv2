namespace LcsServer.Models.LCProjectModels.GlobalBase;

public static class Comparator
{
    public static int GetHashCode<T>(IList<T> list)
    {
        var res = 0x2D2816FE;
        foreach (var item in list)
        {
            res = res * 31 + (item == null ? 0 : item.GetHashCode());
        }
        return res;
    }

    public static bool Equals<T>(IList<T> first, IList<T> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }

        for (var i = 0; i != first.Count; ++i)
        {
            if (!first[i].Equals(second[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static int GetHashCode<TK,TV>(IDictionary<TK, TV> dict)
    {
        var res = 0x2D2816FE;
        foreach (var key in dict.Keys)
        {
            res = res * 31 + key.GetHashCode();
            res = res * 31 + dict[key].GetHashCode();
        }
        return res;
    }

    public static bool Equals<TK,TV>(IDictionary<TK, TV> first, IDictionary<TK, TV> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }

        foreach (var key in first.Keys)
        {
            if (second.TryGetValue(key, out var value))
            {
                if (!first[key].Equals(value))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static bool EqualsOperator<T>(T lhs, T rhs)
    {
        return ReferenceEquals(lhs, null) ? ReferenceEquals(rhs, null) : lhs.Equals(rhs);
    }

}