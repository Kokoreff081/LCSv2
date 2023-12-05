using System.Collections.ObjectModel;
using LcsServer.Models.LCProjectModels.GlobalBase.Addressing;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public abstract class LCDevice : ILCContainer
{
    protected readonly Dictionary<int/*port*/, LCUniverse> _universes = new Dictionary<int, LCUniverse>();

    public ReadOnlyDictionary<int, LCUniverse> Universes => new ReadOnlyDictionary<int, LCUniverse>(_universes);

    public virtual void AddUniverse(LCUniverse universe, int port)
    {
        _universes[port] = universe;
    }

}