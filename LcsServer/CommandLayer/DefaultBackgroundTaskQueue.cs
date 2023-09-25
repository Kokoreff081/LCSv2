using System.Threading.Channels;
using LcsServer.DatabaseLayer;

namespace LcsServer.CommandLayer;

public sealed class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Command> _queue;

    public event IBackgroundTaskQueue.CommandHandling NewCommandAdded;
    public DefaultBackgroundTaskQueue(int capacity)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Command>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(
        Command workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
        NewCommandAdded.Invoke();
    }

    public async ValueTask<Command> DequeueAsync()
    {
        Command workItem = await _queue.Reader.ReadAsync();

        return workItem;
    }
}