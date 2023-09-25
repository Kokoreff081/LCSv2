using System.Reflection;

namespace Acn.Helpers;

public static class ESTACodes
{

    private static readonly Dictionary<string, string> EstaCodes = new Dictionary<string, string>();

    static ESTACodes()
    {
        Assembly asm = Assembly.GetCallingAssembly();
        using Stream stream = asm.GetManifestResourceStream("Acn.manufactures.csv");
        if (stream != null)
        {
            using StreamReader reader = new StreamReader(stream);

            while (reader.ReadLine() is { } line)
            {
                var split = line.Split(';');
                if (split.Length != 2)
                {
                    return;
                }

                string code = split[0].Trim();
                string manufacturer = split[1].Trim();
            
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(manufacturer))
                {
                    return;
                }
                EstaCodes.Add(code, manufacturer);
            }
        }
    }

    public static string GetManufacturerByCode(short estacode)
    {
        return EstaCodes.TryGetValue(estacode.ToString("X4"), out string manufacturer) ? manufacturer : "Unknown";
    }
}