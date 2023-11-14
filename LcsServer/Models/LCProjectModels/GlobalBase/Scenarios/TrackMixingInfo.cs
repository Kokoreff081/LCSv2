using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public class TrackMixingInfo
{
    public TrackMixingInfo(Track track, Mixing mixing = Mixing.Normal, int index = 0)
    {
        Track = track;
        Mixing = mixing;
        Index = index;
    }

    /// <summary>
    /// Трек
    /// </summary>
    public Track Track { get; }

    /// <summary>
    /// Тип смешивания
    /// </summary>
    public Mixing Mixing { get; }

    /// <summary>
    /// Индекс трека в массиве
    /// </summary>
    public int Index { get; }
}