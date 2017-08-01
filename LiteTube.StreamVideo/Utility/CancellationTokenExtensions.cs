using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
  public static class CancellationTokenExtensions
  {
    private static readonly Task PendingTask = (Task) new TaskCompletionSource<object>().Task;
    private static readonly Task CancelledTask;

    static CancellationTokenExtensions()
    {
      TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
      completionSource.TrySetCanceled();
      CancellationTokenExtensions.CancelledTask = (Task) completionSource.Task;
    }

    private static async Task WaitAsync(CancellationToken cancellationToken)
    {
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      using (cancellationToken.Register((Action) (() => TaskEx.Run((Action) (() => tcs.TrySetCanceled())))))
      {
        object obj = await tcs.Task.ConfigureAwait(false);
      }
    }

    public static Task AsTask(this CancellationToken cancellationToken)
    {
      if (!cancellationToken.CanBeCanceled)
        return CancellationTokenExtensions.PendingTask;
      if (cancellationToken.IsCancellationRequested)
        return CancellationTokenExtensions.CancelledTask;
      return CancellationTokenExtensions.WaitAsync(cancellationToken);
    }

    public static void CancelDisposeSafe(this CancellationTokenSource cancellationTokenSource)
    {
      if (null == cancellationTokenSource)
        return;
      CancellationTokenExtensions.CancelSafe(cancellationTokenSource);
      DisposeExtensions.DisposeSafe((IDisposable) cancellationTokenSource);
    }

    public static void CancelSafe(this CancellationTokenSource cancellationTokenSource)
    {
      if (null == cancellationTokenSource)
        return;
      try
      {
        if (!cancellationTokenSource.IsCancellationRequested)
          cancellationTokenSource.Cancel();
      }
      catch (Exception ex)
      {
        Debug.WriteLine("CancellationTokenExtensions.CancelSafe() failed: " + ex.Message);
      }
    }

    public static void BackgroundCancelSafe(this CancellationTokenSource cancellationTokenSource)
    {
      if (null == cancellationTokenSource)
        return;
      try
      {
        if (!cancellationTokenSource.IsCancellationRequested)
        {
          Task task = TaskEx.Run((Action) (() =>
          {
            try
            {
              cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
              Debug.WriteLine("CancellationTokenExtensions.BackgroundCancelSafe() cancel failed: " + ex.Message);
            }
          }));
          TaskCollector.Default.Add(task, "CancellationTokenExtensions BackgroundCancelSafe");
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("CancellationTokenExtensions.BackgroundCancelSafe() failed: " + ex.Message);
      }
    }
  }
}
