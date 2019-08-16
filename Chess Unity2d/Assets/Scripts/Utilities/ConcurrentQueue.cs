using System.Collections.Generic;

/// <summary>
///     Alternative to System.Collections.Concurrent.ConcurrentQueue
///     (It's only available in .NET 4.0 and greater)
/// </summary>
/// <remarks>
///     It's a bit slow (as it uses locks), and only provides a small subset of the interface
///     Overall, the implementation is intended to be simple & robust
/// </remarks>
public class ConcurrentQueue<T>
{
    private readonly Queue<T> queue = new Queue<T>();
    private readonly object queueLock = new object();

    public void Enqueue(T item)
    {
        lock (queueLock)
        {
            queue.Enqueue(item);
        }
    }

    public bool TryDequeue(out T result)
    {
        lock (queueLock)
        {
            if (queue.Count == 0)
            {
                result = default;
                return false;
            }

            result = queue.Dequeue();
            return true;
        }
    }
}