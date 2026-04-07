using System.Runtime.CompilerServices;

namespace ColdWatersModWebApp.Worker;

public sealed class EventHub<T> where T : class
{
    private TaskCompletionSource<T> tcs;

    public EventHub()
    {
        this.tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public void Notify(T eventInstance)
    {
        TaskCompletionSource<T> newTcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<T> oldTcs = Interlocked.Exchange(ref this.tcs, newTcs);
        oldTcs.TrySetResult(eventInstance);
    }

    public async IAsyncEnumerable<T> Subscribe([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            TaskCompletionSource<T> localTcs = this.tcs;

            Task<T> task = localTcs.Task;

            if (task.IsCompleted)
            {
                yield return task.Result;
            }
            else
            {
                using CancellationTokenRegistration reg = cancellationToken.Register(
                    static state => ((TaskCompletionSource<bool>)state!).TrySetCanceled(),
                    localTcs);

                T result;
                try
                {
                    result = await task.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    yield break;
                }

                yield return result;
            }
        }
    }
}
