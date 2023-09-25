using System.Collections.Concurrent;

namespace Acn;

public class DoubleQueue<T>
{
    private readonly ConcurrentQueue<T> _newItems = new ConcurrentQueue<T>();
    private readonly ConcurrentQueue<T> _retryItems = new ConcurrentQueue<T>();

    public void Enqueue(T item, bool isNew)
    {
        if (isNew)
            _newItems.Enqueue(item);
        else
            _retryItems.Enqueue(item);
    }

    public bool TryDequeue(out T result)
    {
        return !_newItems.IsEmpty ? _newItems.TryDequeue(out result) : _retryItems.TryDequeue(out result);
    }
}