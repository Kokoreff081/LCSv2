namespace LcsServer.DevicePollingService.Models;

public static class EstaCodesCache
{
    private static readonly object _lock;
    private static readonly Dictionary<short, string> _estaManufactureDictionary;

    public static bool IsLoaded { get; private set; }

    static EstaCodesCache()
    {
        _lock = new object();
        _estaManufactureDictionary = new Dictionary<short, string>();
    }

    public static Task LoadCacheAsync(string filePath)
    {
        return Task.Factory.StartNew(() => LoadCache(filePath));
    }

    public static Task LoadCacheAsync(StreamReader stream)
    {
        return Task.Factory.StartNew(() => LoadCache(stream));
    }

    private static void LoadCache(StreamReader stream)
    {
        lock (_lock)
        {
            using (stream)
            {
                IsLoaded = false;
                while (stream.Peek() >= 0)
                {
                    string line = stream.ReadLine();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    var estaManufacture = line.Split(new[] { ',' }, 2);
                    if (estaManufacture.Length != 2)
                        return;

                    short estaCode = Convert.ToInt16(estaManufacture[0].Trim(), 16);
                    string manufacture = estaManufacture[1].Trim();

                    _estaManufactureDictionary[estaCode] = manufacture;
                }
                IsLoaded = true;
            }
        }
    }

    private static void LoadCache(string filePath)
    {
        LoadCache(File.OpenText(filePath));
    }

    public static string GetManufactureByEsta(short esta)
    {
        lock (_lock)
        {
            _estaManufactureDictionary.TryGetValue(esta, out string result);
            return result;
        }
    }
}