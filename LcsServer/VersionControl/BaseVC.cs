using System.Reflection;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

namespace LCSVersionControl;

public abstract class BaseVC
{
    private const string ModelAssemblyName = "LcsServer";
    public const string CommonEntitiesAssemblyName = "LcsServer";
    private const string VcAssemblyName = "LcsServer";

    private static readonly Type ModelBaseType = typeof(ISaveLoad);

    /// <summary>
    /// Словарь тип объекта к типу объекта Version control последней версии
    /// </summary>
    public static Dictionary<Type, Type> TypesToVc;

    /// <summary>
    /// Словарь типов объектов Version control к типам объектам
    /// </summary>
    public static Dictionary<Type, Type> TypesFromVc;

    private static readonly Dictionary<string, Assembly> AssemblyByName = new Dictionary<string, Assembly>();

    /// <summary>
    /// Словарь тип объекта к типам всех версий объекта Version control
    /// </summary>
    private static readonly Dictionary<Type, List<Type>> VersionsByBaseVc = new Dictionary<Type, List<Type>>();

    private static readonly Dictionary<VcClassAttribute, Type> VersionTypes;

    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }

    public int Version => GetType().GetCustomAttribute<VcClassAttribute>().Version;
    public string Type => GetType().GetCustomAttribute<VcClassAttribute>().JsonName;

    static BaseVC()
    {
        try
        {
            var vc = GetAssembly(VcAssemblyName).GetTypes()
                .Where(x => x.GetCustomAttribute<VcClassAttribute>() != null).ToList();

            VersionTypes = new Dictionary<VcClassAttribute, Type>();
            foreach (Type type in vc)
            {
                VcClassAttribute attribute = type.GetCustomAttribute<VcClassAttribute>();
                VersionTypes.Add(attribute, type);
            }

            var modelAssembly = GetAssembly(ModelAssemblyName);
            var modelTypes = modelAssembly.GetTypes()
                .Where(p => /*!p.IsAbstract && */p.IsClass && ModelBaseType.IsAssignableFrom(p)).ToArray();

            var commonEntitiesAssembly = GetAssembly(CommonEntitiesAssemblyName);
            var commonEntitiesTypes = commonEntitiesAssembly.GetTypes()
                .Where(p => !p.IsAbstract && p.IsClass && ModelBaseType.IsAssignableFrom(p)).ToArray();


            if (!modelTypes.Any() || !vc.Any() || !commonEntitiesTypes.Any())
            {
                throw new Exception("BaseVC, static constructor: modelTypes or fakeModelTypes is nothing");
            }

            TypesToVc = new Dictionary<Type, Type>();
            TypesFromVc = new Dictionary<Type, Type>();

            foreach (Type[] types in new[] { modelTypes/*, commonEntitiesTypes */})
            {
                foreach (Type t in types)
                {
                    var vsTypes = VersionTypes.Where(x =>
                        x.Key.JsonName.Equals(t.Name, StringComparison.InvariantCultureIgnoreCase)).ToArray();

                    List<Type> versions =
                        vsTypes.OrderBy(x => x.Key.Version).Select(x => x.Value)
                            .ToList(); // Отсортировать список по версии

                    Type lastVersion = versions.LastOrDefault();
                    if (lastVersion == null)
                    {
                        continue;
                    }

                    TypesToVc[t] = lastVersion;

                    VersionsByBaseVc.Add(t, versions);

                    foreach (var vsType in vsTypes)
                    {
                        TypesFromVc[vsType.Value] = t;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static Assembly GetAssembly(string assemblyName)
    {
        if (AssemblyByName.TryGetValue(assemblyName, out Assembly assembly1))
        {
            return assembly1;
        }

        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);
        if (assembly == null)
        {
            throw new Exception($"BaseVC.GetAssembly: \"{assemblyName}\" is not found");
        }

        AssemblyByName.Add(assemblyName, assembly);

        return assembly;

    }

    public static BaseVC CreateBaseVC(string jsonClassName, int version)
    {
        if (!VersionTypes.TryGetValue(new VcClassAttribute { JsonName = jsonClassName, Version = version },
                out Type baseVcType))
        {
            return null;
        }

        return Activator.CreateInstance(baseVcType) as BaseVC;
    }

    /// <summary>
    /// Конвертирует объект в соотвествующий ему объект VersionControl
    /// </summary>
    /// <param name="o">Объект для конвертирования в VersionControl</param>
    /// <returns>Соответствующий объект VersionControl</returns>
    public static BaseVC CreateFromConcreteObject(ISaveLoad o)
    {
        Type inType = o.GetType();
        //Type outType = TypesToVc[inType];
        if (!TypesToVc.TryGetValue(inType, out Type outType))
        {
            throw new ArgumentException($"VCObject not found type {inType}");
        }

        if (Activator.CreateInstance(outType) is not BaseVC obj)
        {
            throw new ArgumentException($"VCObject creation error {outType}");
        }

        BaseVC result = obj.FromConcreteObject(o);
        if (obj == null)
        {
            throw new ArgumentException($"FromConcreteObject error {o}");
        }

        return result;
    }

    /// <summary>
    /// Конвертирует объект VersionControl до его последней актуальной версии
    /// </summary>
    /// <param name="vcItem">Объект VersionControl</param>
    /// <returns>Сконвертированный объект до последней актуальной версии</returns>
    public static BaseVC VCItemToLastVersion(BaseVC vcItem)
    {
        try
        {
            IEnumerable<Type> versions = VersionsByBaseVc[TypesFromVc[vcItem.GetType()]];

            versions = versions.Where(x =>
                x.GetCustomAttribute<VcClassAttribute>()?.Version >
                vcItem.Version); // Включить только более раннии версии

            // if (!versions.Any())
            //     return vcItem;

            BaseVC lastVcItemVersion = vcItem;

            foreach (Type version in versions)
            {
                BaseVC nextVersion = (BaseVC)Activator.CreateInstance(version);
                if (nextVersion != null)
                {
                    nextVersion.FromPrevious(lastVcItemVersion);
                    lastVcItemVersion = nextVersion;
                }
            }

            return lastVcItemVersion;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Возвращает объект сконвертированный из объекта VersionControl
    /// </summary>
    /// <returns></returns>
    public abstract ISaveLoad ToConcreteObject();

    /// <summary>
    /// Возвращает объект VersionControl
    /// </summary>
    /// <param name="o">Объект для конвертирования в VersionControl</param>
    /// <returns></returns>
    public abstract BaseVC FromConcreteObject(ISaveLoad o);

    /// <summary>
    /// Конвертирует объект VersionControl из предыдущей версии
    /// </summary>
    /// <param name="baseVC"></param>
    public virtual void FromPrevious(BaseVC baseVC)
    {
        Id = baseVC.Id;
        Name = baseVC.Name;
        ParentId = baseVC.ParentId;
    }
}
