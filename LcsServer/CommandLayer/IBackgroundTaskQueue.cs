using LcsServer.DatabaseLayer;

namespace LcsServer.CommandLayer;

public interface IBackgroundTaskQueue
{
    delegate void CommandHandling();
    public event CommandHandling NewCommandAdded;
    ValueTask QueueBackgroundWorkItemAsync(Command workItem);

    ValueTask<Command> DequeueAsync();
}