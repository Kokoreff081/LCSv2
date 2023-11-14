using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

namespace LCSVersionControl.Interfaces;

public interface ISequence<T> where T : LCObject
{
    event EventHandler<SequenceEventArgs> SequenceChanged;

    IReadOnlyDictionary<long, T> StartToEntityDictionary { get; }
    IReadOnlyList<Transition> Transitions { get; }
    IReadOnlyList<T> Entities { get; }

    bool TryGetTransition(int playItem1Id, int playItem2Id, out Transition transition);
    bool TryGetPlayItem(int playItemId, out IPlayingEntity playItem, out long start);
    IEnumerable<IPlayingEntity> GetSequence();
    long GetStartOfPlayItem(int itemId);
    void ChangeSequence(
        IEnumerable<KeyValuePair<T, long>> addEntities,
        IEnumerable<KeyValuePair<T, long>> removeEntities,
        IEnumerable<Transition> addTransition,
        IEnumerable<Transition> removeTransition,
        IEnumerable<KeyValuePair<T, long>> changeTEntityToStart);
}