using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
  public static class TplTaskExtensions
  {
    public static readonly Task NeverCompletedTask = (Task) new TaskCompletionSource<object>().Task;
    public static readonly Task<bool> TrueTask = TaskEx.FromResult<bool>(true);
    public static readonly Task<bool> FalseTask = TaskEx.FromResult<bool>(false);
    public static readonly Task CompletedTask = (Task) TplTaskExtensions.TrueTask;

    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
      TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
      using (cancellationToken.Register((Action<object>) (s => ((TaskCompletionSource<bool>) s).TrySetResult(true)), (object) tcs, false))
      {
        if ((Task<T>) task != await TaskEx.WhenAny((Task) task, (Task) tcs.Task).ConfigureAwait(false))
          throw new OperationCanceledException(cancellationToken);
      }
      return await task.ConfigureAwait(false);
    }

    public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
    {
      TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
      using (cancellationToken.Register((Action<object>) (s => ((TaskCompletionSource<bool>) s).TrySetResult(true)), (object) tcs, false))
      {
        if (task != await TaskEx.WhenAny(task, (Task) tcs.Task).ConfigureAwait(false))
          throw new OperationCanceledException(cancellationToken);
      }
      await task.ConfigureAwait(false);
    }
  }
}
